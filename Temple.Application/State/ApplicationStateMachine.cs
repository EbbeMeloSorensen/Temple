using Stateless;

namespace Temple.Application.State;

public class ApplicationStateMachine
{
    internal readonly StateMachine<ApplicationStateType, Trigger> _machine;

    public ApplicationState CurrentScene { get; private set; }

    public event Action<ApplicationState>? SceneChanged;

    public ApplicationStateMachine()
    {
        _machine = new StateMachine<ApplicationStateType, Trigger>(ApplicationStateType.Starting);

        _machine.Configure(ApplicationStateType.Starting)
            .Permit(Trigger.Initialize, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.MainMenu)
            .Permit(Trigger.GoToSmurfManagement, ApplicationStateType.SmurfManagement)
            .Permit(Trigger.GoToPeopleManagement, ApplicationStateType.PeopleManagement)
            .Permit(Trigger.StartNewGame, ApplicationStateType.Intro)
            .Permit(Trigger.ShutdownRequested, ApplicationStateType.ShuttingDown);

        _machine.Configure(ApplicationStateType.SmurfManagement)
            .Permit(Trigger.ExitState, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.PeopleManagement)
            .Permit(Trigger.ExitState, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.Intro)
            .Permit(Trigger.ExitState, ApplicationStateType.Battle_First);

        _machine.Configure(ApplicationStateType.Battle_First)
            .Permit(Trigger.ExitState, ApplicationStateType.ExploreArea_AfterFirstBattle)
            .Permit(Trigger.GoToDefeat, ApplicationStateType.Defeat);

        _machine.Configure(ApplicationStateType.ExploreArea_AfterFirstBattle)
            .Permit(Trigger.ExitState, ApplicationStateType.Battle_Final);

        _machine.Configure(ApplicationStateType.Battle_Final)
            .Permit(Trigger.ExitState, ApplicationStateType.Victory)
            .Permit(Trigger.GoToDefeat, ApplicationStateType.Defeat);

        _machine.Configure(ApplicationStateType.Defeat)
            .Permit(Trigger.ExitState, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.Victory)
            .Permit(Trigger.ExitState, ApplicationStateType.MainMenu);

        CurrentScene = new ApplicationState(
            _machine.State);
    }

    private void ChangeScene(ApplicationState scene)
    {
        CurrentScene = scene;
        SceneChanged?.Invoke(scene);
    }

    public void Fire(Trigger trigger)
    {
        if (_machine.CanFire(trigger))
        {
            _machine.Fire(trigger);

            ChangeScene(new ApplicationState(_machine.State));
        }
        else
        {
            Console.WriteLine($"Ignored trigger {trigger} in state {_machine.State}");
        }
    }
}