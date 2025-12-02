using Stateless;

namespace Temple.Application.State;

public class ApplicationStateMachine
{
    internal readonly StateMachine<ApplicationStateType, ApplicationStateShiftTrigger> _machine;

    public ApplicationState CurrentState { get; private set; }

    public event Action<ApplicationState>? StateChanged;

    public ApplicationStateMachine()
    {
        _machine = new StateMachine<ApplicationStateType, ApplicationStateShiftTrigger>(ApplicationStateType.Starting);

        _machine.Configure(ApplicationStateType.Starting)
            .Permit(ApplicationStateShiftTrigger.Initialize, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.MainMenu)
            .Permit(ApplicationStateShiftTrigger.GoToSmurfManagement, ApplicationStateType.SmurfManagement)
            .Permit(ApplicationStateShiftTrigger.GoToPeopleManagement, ApplicationStateType.PeopleManagement)
            .Permit(ApplicationStateShiftTrigger.StartNewGame, ApplicationStateType.Intro)
            .Permit(ApplicationStateShiftTrigger.ShutdownRequested, ApplicationStateType.ShuttingDown);

        _machine.Configure(ApplicationStateType.SmurfManagement)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.PeopleManagement)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.Intro)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.Battle_First);

        _machine.Configure(ApplicationStateType.Battle_First)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.ExploreArea_AfterFirstBattle)
            .Permit(ApplicationStateShiftTrigger.GoToDefeat, ApplicationStateType.Defeat);

        _machine.Configure(ApplicationStateType.ExploreArea_AfterFirstBattle)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.Battle_Final);

        _machine.Configure(ApplicationStateType.Battle_Final)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.Victory)
            .Permit(ApplicationStateShiftTrigger.GoToDefeat, ApplicationStateType.Defeat);

        _machine.Configure(ApplicationStateType.Defeat)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.MainMenu);

        _machine.Configure(ApplicationStateType.Victory)
            .Permit(ApplicationStateShiftTrigger.ExitState, ApplicationStateType.MainMenu);

        CurrentState = new ApplicationState(
            _machine.State);
    }

    private void ChangeScene(ApplicationState scene)
    {
        CurrentState = scene;
        StateChanged?.Invoke(scene);
    }

    public void Fire(ApplicationStateShiftTrigger applicationStateShiftTrigger)
    {
        if (_machine.CanFire(applicationStateShiftTrigger))
        {
            _machine.Fire(applicationStateShiftTrigger);

            ChangeScene(new ApplicationState(_machine.State));
        }
        else
        {
            Console.WriteLine($"Ignored trigger {applicationStateShiftTrigger} in state {_machine.State}");
        }
    }
}