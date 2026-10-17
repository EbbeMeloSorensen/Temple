using Craft.Logging;
using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Microsoft.Extensions.Logging.Abstractions;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Entities.DD.Quests;
using Temple.ViewModel.DD.Exploration;
using Temple.ViewModel.DD.Exploration.Bodies;
using Xunit;

namespace Temple.ViewModel.UnitTest;

public class ExplorationInputTests
{
    private static ApplicationController CreateController()
    {
        var machine = new ApplicationStateMachine();
        var controller = new ApplicationController(machine, null!, new EmptyGameIO(),
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
