using Craft.Math;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;

namespace Temple.ViewModel.DD.Exploration;

internal sealed class DoorGeometryCache
{
    private readonly Dictionary<BodyDoor, Entry> _entries = new();
    private readonly HashSet<BodyDoor> _seen = new();

    private sealed record Entry(
        (double X1, double Y1, double X2, double Y2, double Opening, bool Clockwise) State,
        LineSegment2D Geometry);

    public void BeginFrame() => _seen.Clear();

    public LineSegment2D GetGeometry(BodyStateDoor state)
    {
        var door = (BodyDoor)state.Body;
        _seen.Add(door);
        var key = (door.Point1.X, door.Point1.Y, door.Point2.X, door.Point2.Y,
            state.PercentageOpen, state.OpenClockWise);
        if (_entries.TryGetValue(door, out var entry) && entry.State == key)
        {
            return entry.Geometry;
        }

        var angle = state.PercentageOpen * 0.5 * Math.PI / 100;
        var doorVector = door.Point2 - door.Point1;
        var perpendicular = doorVector.Hat();
        if (!state.OpenClockWise)
        {
            perpendicular = -perpendicular;
        }

        var geometry = new LineSegment2D(
            new Point2D(door.Point1.X, door.Point1.Y),
            new Point2D(
                door.Point1.X + Math.Cos(angle) * doorVector.X + Math.Sin(angle) * perpendicular.X,
                door.Point1.Y + Math.Cos(angle) * doorVector.Y + Math.Sin(angle) * perpendicular.Y));
        _entries[door] = new Entry(key, geometry);
        return geometry;
    }

    public void EndFrame()
    {
        if (_seen.Count == _entries.Count)
        {
            return;
        }

        foreach (var door in _entries.Keys.Where(door => !_seen.Contains(door)).ToArray())
        {
            _entries.Remove(door);
        }
    }

    public void Clear()
    {
        _entries.Clear();
        _seen.Clear();
    }
}
