using Stateless;

namespace Temple.Application.State;

public class ApplicationStateMachine
{
    internal readonly StateMachine<StateMachineState, ApplicationStateShiftTrigger> _machine;

    public ApplicationState CurrentState { get; private set; }

    public event Action<ApplicationState>? StateChanged;

    public ApplicationStateMachine()
    {
        _machine = new StateMachine<StateMachineState, ApplicationStateShiftTrigger>(StateMachineState.Starting);

        _machine.Configure(StateMachineState.Starting)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.Initialize, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.MainMenu)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.GoToSmurfManagement, StateMachineState.SmurfManagement)
            .Permit(ApplicationStateShiftTrigger.GoToPeopleManagement, StateMachineState.PeopleManagement)
            .Permit(ApplicationStateShiftTrigger.StartNewGame, StateMachineState.Intro)
            .Permit(ApplicationStateShiftTrigger.ShutdownRequested, StateMachineState.ShuttingDown);

        _machine.Configure(StateMachineState.SmurfManagement)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.PeopleManagement)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.Intro)
            .OnEntry(() =>
            {
                // Denne payload skal specificere, at det er intro-prelude, der skal vises
                var dummyPayload = new ApplicationStatePayload {JustAString = "Intro"};
                var applicationState = new ApplicationState(_machine.State, StateMachineStateType.Interlude, dummyPayload);
                UpdateApplicationState(applicationState);
            })
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.ExploreArea_Dungeon1);

        _machine.Configure(StateMachineState.ExploreArea_Dungeon1)
            .OnEntry(() =>
            {
                var dummyPayload = new ApplicationStatePayload{JustAString = "Dungeon1"};
                var applicationState = new ApplicationState(_machine.State, StateMachineStateType.Exploration, dummyPayload);
                UpdateApplicationState(applicationState);
            })
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.Battle);

        _machine.Configure(StateMachineState.Battle)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.ExploreArea_Dungeon1)
            .Permit(ApplicationStateShiftTrigger.GoToDefeat, StateMachineState.Defeat)
            .Permit(ApplicationStateShiftTrigger.GoToVictory, StateMachineState.Victory);

        _machine.Configure(StateMachineState.Defeat)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.Victory)
            .OnEntry(() => UpdateApplicationState())
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        CurrentState = new ApplicationState(
            _machine.State);
    }

    private void UpdateApplicationState(ApplicationState? state = null)
    {
        state ??= new ApplicationState(_machine.State);
        CurrentState = state;
        StateChanged?.Invoke(state);
    }

    public void Fire(ApplicationStateShiftTrigger applicationStateShiftTrigger)
    {
        if (_machine.CanFire(applicationStateShiftTrigger))
        {
            _machine.Fire(applicationStateShiftTrigger);
        }
        else
        {
            Console.WriteLine($"Ignored trigger {applicationStateShiftTrigger} in state {_machine.State}");
        }
    }
}