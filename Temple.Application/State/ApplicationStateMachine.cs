using Stateless;
using Temple.Application.State.Payloads;

namespace Temple.Application.State;

public class ApplicationStateMachine
{
    internal readonly StateMachine<StateMachineState, ApplicationStateShiftTrigger> _machine;

    public ApplicationState CurrentState { get; private set; }
    public ApplicationStatePayload NextPayload { get; internal set; }

    public event Action<ApplicationState>? StateChanged;

    public ApplicationStateMachine()
    {
        _machine = new StateMachine<StateMachineState, ApplicationStateShiftTrigger>(StateMachineState.Starting);

        _machine.Configure(StateMachineState.Starting)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.Initialize, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.MainMenu)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.GoToSmurfManagement, StateMachineState.SmurfManagement)
            .Permit(ApplicationStateShiftTrigger.GoToPeopleManagement, StateMachineState.PeopleManagement)
            .Permit(ApplicationStateShiftTrigger.StartNewGame, StateMachineState.Interlude)
            .Permit(ApplicationStateShiftTrigger.ShutdownRequested, StateMachineState.ShuttingDown);

        _machine.Configure(StateMachineState.SmurfManagement)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.PeopleManagement)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.Interlude)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.GoToExploration, StateMachineState.Exploration);

        _machine.Configure(StateMachineState.Exploration)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.GoToBattle, StateMachineState.Battle)
            .Permit(ApplicationStateShiftTrigger.GoToWilderness, StateMachineState.Wilderness);

        _machine.Configure(StateMachineState.Wilderness)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.GoToBattle, StateMachineState.Battle)
            .Permit(ApplicationStateShiftTrigger.GoToExploration, StateMachineState.Exploration);

        _machine.Configure(StateMachineState.Battle)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.GoToExploration, StateMachineState.Exploration)
            .Permit(ApplicationStateShiftTrigger.GoToWilderness, StateMachineState.Wilderness)
            .Permit(ApplicationStateShiftTrigger.GoToDefeat, StateMachineState.Defeat)
            .Permit(ApplicationStateShiftTrigger.GoToVictory, StateMachineState.Victory);

        _machine.Configure(StateMachineState.Defeat)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.Victory)
            .OnEntry(UpdateApplicationState)
            .Permit(ApplicationStateShiftTrigger.ExitState, StateMachineState.MainMenu);

        _machine.Configure(StateMachineState.ShuttingDown)
            .OnEntry(UpdateApplicationState);

        CurrentState = new ApplicationState(
            _machine.State);
    }

    private void UpdateApplicationState()
    {
        // Her skal vi have fat i den næste payload. Den kommunikeres fra ...
        // I første omgang hardkoder vi lige en
 
        var state = new ApplicationState(_machine.State, NextPayload);
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