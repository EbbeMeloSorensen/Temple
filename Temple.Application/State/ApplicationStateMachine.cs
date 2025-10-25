using Stateless;

namespace Temple.Application.State;

public class ApplicationStateMachine
{
    internal readonly StateMachine<ApplicationState, ApplicationTrigger> _machine;
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
            .Permit(ApplicationTrigger.Initialize, ApplicationState.MainMenu)
            .OnExit(() => RaiseStateChanged(ApplicationState.Starting, ApplicationState.MainMenu));

        _machine.Configure(ApplicationState.MainMenu)
            .Permit(ApplicationTrigger.ShutdownRequested, ApplicationState.ShuttingDown);

        _machine.Configure(ApplicationState.MainMenu)
            .Permit(ApplicationTrigger.GoToSmurfManagement, ApplicationState.SmurfManagement)
            .Permit(ApplicationTrigger.GoToPeopleManagement, ApplicationState.PeopleManagement)
            .Permit(ApplicationTrigger.StartNewGame, ApplicationState.Intro);

        // Under construction..
        _machine.Configure(ApplicationState.Intro)
            .OnExit(() => RaiseStateChanged(ApplicationState.Intro, ApplicationState.FirstBattle));

        _machine.Configure(ApplicationState.SmurfManagement)
            .Permit(ApplicationTrigger.GoToHome, ApplicationState.MainMenu);

        _machine.Configure(ApplicationState.PeopleManagement)
            .Permit(ApplicationTrigger.GoToHome, ApplicationState.MainMenu);

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
