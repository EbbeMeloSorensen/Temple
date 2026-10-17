using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Temple.ViewModel.DD.Exploration;
using Xunit;

namespace Temple.ViewModel.UnitTest;

public class DoorPresentationTests
{
    private static BodyStateDoor CreateDoor(int id = 100) =>
        new(new BodyDoor(id, 1, false, false, null)
        {
            Point1 = new Vector2D(0, 0),
            Point2 = new Vector2D(0, 2)
        }, true, 0);

    private static LineSegment2D Frame(DoorGeometryCache cache, BodyStateDoor state)
    {
        cache.BeginFrame();
        var geometry = cache.GetGeometry(state);
        cache.EndFrame();
        return geometry;
    }

    [Fact]
    public void UnchangedDoor_ReusesGeometryAcrossSimulationStates()
    {
        var cache = new DoorGeometryCache();
        var state = CreateDoor();
        var initial = Frame(cache, state);
        for (var frame = 0; frame < 100; frame++)
        {
            Assert.Same(initial, Frame(cache, (BodyStateDoor)state.Clone()));
        }
    }

    [Theory]
    [InlineData(true, -2)]
    [InlineData(false, 2)]
    public void OpeningDoor_UpdatesEndpointAndThenReusesGeometry(bool clockwise, double expectedX)
    {
        var cache = new DoorGeometryCache();
        var state = CreateDoor();
        var closed = Frame(cache, state);
        state.OpenClockWise = clockwise;
        state.PercentageOpen = 100;
        var open = Frame(cache, state);
        Assert.NotSame(closed, open);
        Assert.Equal(expectedX, open.Point2.X, 10);
        Assert.Equal(0, open.Point2.Y, 10);
        Assert.Same(open, Frame(cache, state));
    }

    [Fact]
    public void OpeningOneDoor_PreservesOtherDoorGeometry()
    {
        var cache = new DoorGeometryCache();
        var first = CreateDoor();
        var second = CreateDoor(101);
        cache.BeginFrame();
        var originalFirst = cache.GetGeometry(first);
        var originalSecond = cache.GetGeometry(second);
        cache.EndFrame();
        first.PercentageOpen = 50;
        cache.BeginFrame();
        Assert.NotSame(originalFirst, cache.GetGeometry(first));
        Assert.Same(originalSecond, cache.GetGeometry(second));
        cache.EndFrame();
    }

    [Fact]
    public void RemovedDoor_IsEvictedFromCache()
    {
        var cache = new DoorGeometryCache();
        var state = CreateDoor();
        var original = Frame(cache, state);
        cache.BeginFrame();
        cache.EndFrame();
        Assert.NotSame(original, Frame(cache, state));
    }

    [Fact]
    public void NewSite_DoesNotReuseGeometryFromPreviousSite()
    {
        var cache = new DoorGeometryCache();
        var state = CreateDoor();
        var original = Frame(cache, state);
        cache.Clear();
        Assert.NotSame(original, Frame(cache, state));
    }

    [Fact]
    public void Rotation_OnlyNotifiesWhenAngleChanges()
    {
        var rotation = new DoorRotationViewModel();
        var notifications = 0;
        rotation.PropertyChanged += (_, args) =>
        {
            Assert.Equal(nameof(DoorRotationViewModel.RotationAngle), args.PropertyName);
            notifications++;
        };
        rotation.RotationAngle = 0;
        rotation.RotationAngle = 45;
        rotation.RotationAngle = 45;
        rotation.RotationAngle = 90;
        Assert.Equal(2, notifications);
    }
}
