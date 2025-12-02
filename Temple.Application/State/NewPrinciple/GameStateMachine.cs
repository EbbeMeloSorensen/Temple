using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Stateless;
using Stateless.Graph;

namespace Temple.Application.State.NewPrinciple;

public class GameStateMachine
{
    internal readonly StateMachine<SceneType, Trigger> _machine;

    public GameScene CurrentScene { get; private set; }

    public event Action<GameScene>? SceneChanged;

    public GameStateMachine()
    {
        _machine = new StateMachine<SceneType, Trigger>(SceneType.Starting);

        _machine.Configure(SceneType.Starting)
            .Permit(Trigger.Initialize, SceneType.MainMenu);

        _machine.Configure(SceneType.MainMenu)
            .Permit(Trigger.GoToSmurfManagement, SceneType.SmurfManagement)
            .Permit(Trigger.GoToPeopleManagement, SceneType.PeopleManagement)
            .Permit(Trigger.StartNewGame, SceneType.Intro)
            .Permit(Trigger.ShutdownRequested, SceneType.ShuttingDown);

        _machine.Configure(SceneType.SmurfManagement)
            .Permit(Trigger.ExitState, SceneType.MainMenu);

        _machine.Configure(SceneType.PeopleManagement)
            .Permit(Trigger.ExitState, SceneType.MainMenu);

        _machine.Configure(SceneType.Intro)
            .Permit(Trigger.ExitState, SceneType.Battle_First);

        _machine.Configure(SceneType.Battle_First)
            .Permit(Trigger.ExitState, SceneType.ExploreArea_AfterFirstBattle)
            .Permit(Trigger.GoToDefeat, SceneType.Defeat);

        _machine.Configure(SceneType.ExploreArea_AfterFirstBattle)
            .Permit(Trigger.ExitState, SceneType.Battle_Final);

        _machine.Configure(SceneType.Battle_Final)
            .Permit(Trigger.ExitState, SceneType.Victory)
            .Permit(Trigger.GoToDefeat, SceneType.Defeat);

        _machine.Configure(SceneType.Defeat)
            .Permit(Trigger.ExitState, SceneType.MainMenu);

        _machine.Configure(SceneType.Victory)
            .Permit(Trigger.ExitState, SceneType.MainMenu);

        CurrentScene = new GameScene(
            _machine.State);
    }

    private void ChangeScene(GameScene scene)
    {
        CurrentScene = scene;
        SceneChanged?.Invoke(scene);
    }

    public void Fire(Trigger trigger)
    {
        if (_machine.CanFire(trigger))
        {
            _machine.Fire(trigger);

            ChangeScene(new GameScene(_machine.State));
        }
        else
        {
            Console.WriteLine($"Ignored trigger {trigger} in state {_machine.State}");
        }
    }
}