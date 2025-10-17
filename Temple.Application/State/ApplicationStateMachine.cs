using Stateless;

namespace Temple.Application.State;

public class ApplicationStateMachine
{
    private readonly StateMachine<ApplicationState, ApplicationTrigger> _machine;
    private ApplicationState _state;

    public ApplicationState CurrentState => _state;

    public event EventHandler<ApplicationStateChangedEventArgs> StateChanged;

    public ApplicationStateMachine()
    {
        _machine = new StateMachine<ApplicationState, ApplicationTrigger>(
            () => _state,
            s => _state = s
        );

        Configure();
    }

    private void Configure()
    {
        _machine.Configure(ApplicationState.Starting)
            .Permit(ApplicationTrigger.Initialize, ApplicationState.Idle)
            .OnExit(() => RaiseStateChanged(ApplicationState.Starting, ApplicationState.Idle));

        _machine.Configure(ApplicationState.Idle)
            .Permit(ApplicationTrigger.WorkRequested, ApplicationState.Working)
            .Permit(ApplicationTrigger.ShutdownRequested, ApplicationState.ShuttingDown);

        _machine.Configure(ApplicationState.Working)
            .Permit(ApplicationTrigger.WorkCompleted, ApplicationState.Idle)
            .Permit(ApplicationTrigger.ErrorOccurred, ApplicationState.Error)
            .OnExit(() => RaiseStateChanged(ApplicationState.Working, _state));

        _machine.Configure(ApplicationState.Error)
            .Permit(ApplicationTrigger.ShutdownRequested, ApplicationState.ShuttingDown)
            .Permit(ApplicationTrigger.Initialize, ApplicationState.Idle);

        _machine.Configure(ApplicationState.ShuttingDown)
            .OnEntry(() => RaiseStateChanged(ApplicationState.ShuttingDown, ApplicationState.ShuttingDown))
            .Ignore(ApplicationTrigger.ShutdownRequested);
    }

    public void Fire(ApplicationTrigger trigger)
    {
        if (_machine.CanFire(trigger))
        {
            var oldState = _state;
            _machine.Fire(trigger);
            if (oldState != _state)
                RaiseStateChanged(oldState, _state);
        }
        else
        {
            Console.WriteLine($"Ignored trigger {trigger} in state {_state}");
        }
    }

    private void RaiseStateChanged(ApplicationState oldState, ApplicationState newState)
        => StateChanged?.Invoke(this, new ApplicationStateChangedEventArgs(oldState, newState));
}
