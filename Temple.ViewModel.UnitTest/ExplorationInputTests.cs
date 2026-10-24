using Temple.Application.Saves;
using Craft.Logging;
using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Microsoft.Extensions.Logging.Abstractions;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Quests;
using Temple.ViewModel.DD.Exploration;
using Temple.ViewModel.DD.Exploration.Bodies;
using Xunit;

namespace Temple.ViewModel.UnitTest;

public class ExplorationInputTests
{
    [Theory]
    [InlineData(1.524, 0, 0.75)]
    [InlineData(2, 90, 0.6)]
    [InlineData(3, 37, 0.9)]
    [InlineData(1, 0, 0.75)]
    [InlineData(0.5, 0, 0.75)]
    public void DoorFillersUseOnlyRatioForWidthAndOuterSpan(double ratio, double orientation, double doorWidth)
    {
        var site = new SiteData { StartPosition = new Point2D(0, 0), StartOrientation = 90 };
        site.SiteComponents.Add(new Door
        {
            Id = "test", Position = new Vector3D(4, 6, 0), Width = doorWidth, Orientation = orientation
        });
        var scene = ExplorationSceneFactory.GenerateScene(site, CreateController(), new EmptyQueries(), ratio);
        var lines = scene.Boundaries.OfType<Craft.Simulation.Boundaries.LineSegment>().ToList();
        var door = Assert.IsType<BodyDoor>(scene.InitialState.TryGetBodyState(100).Body);
        Assert.Equal(doorWidth, (door.Point2 - door.Point1).Length, 10);
        if (ratio <= 1)
        {
            Assert.Empty(lines);
            return;
        }

        Assert.Equal(2, lines.Count);
        Assert.All(lines, line => Assert.Equal((ratio - 1) / 2, (line.Point2 - line.Point1).Length, 10));
        Assert.Equal(ratio, (lines[1].Point2 - lines[0].Point1).Length, 10);
        Assert.Equal(1, (lines[1].Point1 - lines[0].Point2).Length, 10);
        Assert.Equal(4 * ratio, (lines[0].Point1.X + lines[1].Point2.X) / 2, 10);
        Assert.Equal(-6 * ratio, (lines[0].Point1.Y + lines[1].Point2.Y) / 2, 10);

        // Unrelated wall endpoints must not change the fillers.
        site.AddWall(new[] { new Point2D(-20, 6), new Point2D(20, 6) });
        var withWall = ExplorationSceneFactory.GenerateScene(site, CreateController(), new EmptyQueries(), ratio);
        var unchanged = withWall.Boundaries.OfType<Craft.Simulation.Boundaries.LineSegment>().Take(2).ToList();
        for (var i = 0; i < 2; i++)
        {
            Assert.Equal(lines[i].Point1.X, unchanged[i].Point1.X);
            Assert.Equal(lines[i].Point2.Y, unchanged[i].Point2.Y);
        }
    }

    private sealed class EmptyQueries : IGameQueryService
    {
        public bool IsFactEstablished(string factId) => false;
        public bool IsKnowledgeGained(string knowledgeId) => false;
        public bool DoesQuestStatusEqualRequiredValue(string questId, QuestStatus status) => false;
        public bool IsBattleWon(string battleId) => false;
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.524)]
    [InlineData(2.0)]
    public void SiteEntryScalesMapPositionOnceAndReentryPreservesSavedWorldPosition(double sizeRatio)
    {
        var controller = CreateController();
        var site = new SiteData { StartPosition = new Point2D(2, 3), StartOrientation = 90 };
        var scene = ExplorationSceneFactory.GenerateScene(site, controller, null!, sizeRatio);
        var player = scene.InitialState.TryGetBodyState(1);
        Assert.Equal(2 * sizeRatio, player.Position.X);
        Assert.Equal(-3 * sizeRatio, player.Position.Y);

        // Movement saves world coordinates with the site's positive-Y convention.
        controller.ApplicationData.ExplorationPosition = new Vector2D(7, 11);
        var reentered = ExplorationSceneFactory.GenerateScene(site, controller, null!, sizeRatio);
        var returnedPlayer = reentered.InitialState.TryGetBodyState(1);
        Assert.Equal(7, returnedPlayer.Position.X);
        Assert.Equal(-11, returnedPlayer.Position.Y);
    }

    private static ApplicationController CreateController()
    {
        var machine = new ApplicationStateMachine();
        var controller = new ApplicationController(machine, null!, new GameSessionFactory(new EmptyGameIO(), new GameSaveValidator()), null!, new GameSaveValidator(),
            NullLogger<ApplicationController>.Instance);
        machine.Fire(ApplicationStateShiftTrigger.Initialize);
        controller.StartNewGame();
        return controller;
    }

    private static Scene CreateSite(ApplicationController controller) =>
        ExplorationSceneFactory.GenerateScene(new SiteData
        {
            StartPosition = new Point2D(0, 0),
            StartOrientation = 90
        }, controller, null!);

    [Fact]
    public void HoldingUpAcrossSiteRestartMovesPlayerWithoutAnotherPress()
    {
        var controller = CreateController();
        var core = new EngineCore(new DummyLogger());
        var held = new KeyboardState { UpArrowDown = true };
        try
        {
            for (var site = 0; site < 2; site++)
            {
                core.Scene = CreateSite(controller);
                core.SpawnNewThread();
                var state = core.TryGetState(0, held, new KeyboardState(), null, out _);
                var player = Assert.IsType<BodyStateClassic>(state.TryGetBodyState(1));
                Assert.Equal(3, player.ArtificialVelocity.X);

                if (site == 0) core.Reset();
            }

            // Release only after crossing the boundary with Up continuously held.
            var released = core.TryGetState(0, new KeyboardState(), new KeyboardState(), null, out _);
            var stoppedPlayer = Assert.IsType<BodyStateClassic>(released.TryGetBodyState(1));
            Assert.Equal(0, stoppedPlayer.ArtificialVelocity.Length);
        }
        finally
        {
            core.Reset();
        }
    }

    [Fact]
    public void SpaceTapReleasedBeforeUpdateStillLaunchesOneInteractionProbe()
    {
        var scene = CreateSite(CreateController());
        var state = scene.InitialState.Clone();
        var presses = new KeyPressBuffer();
        presses.Enqueue(KeyboardKey.Space);
        scene.InteractionCallBack(new KeyboardState(), presses.Peek(), null, new(), state);
        presses.Consume();
        scene.PostPropagationCallBack(state, new(), new());
        Assert.Single(state.BodyStates.Where(body => body.Body is Probe));

        scene.InteractionCallBack(new KeyboardState(), presses.Peek(), null, new(), state);
        scene.PostPropagationCallBack(state, new(), new());
        Assert.Single(state.BodyStates.Where(body => body.Body is Probe));
    }

    private sealed class EmptyGameIO : IGameIOHandler
    {
        public IEnumerable<string> ReadSiteIdsFromDirectory(string directoryPath) => Array.Empty<string>();
        public IEnumerable<Quest> ReadQuestListFromFile(string fileName) => Array.Empty<Quest>();
        public void WriteQuestsToFile(IEnumerable<Quest> quests, string fileName) => throw new NotSupportedException();
    }
}
