using Stateless;

namespace Temple.Application.State.OldPrinciple;

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
            .Permit(ApplicationTrigger.Initialize, ApplicationState.MainMenu);

        _machine.Configure(ApplicationState.MainMenu)
            .Permit(ApplicationTrigger.ShutdownRequested, ApplicationState.ShuttingDown);

        _machine.Configure(ApplicationState.MainMenu)
            .Permit(ApplicationTrigger.GoToSmurfManagement, ApplicationState.SmurfManagement)
            .Permit(ApplicationTrigger.GoToPeopleManagement, ApplicationState.PeopleManagement)
            .Permit(ApplicationTrigger.StartNewGame, ApplicationState.Intro);

        _machine.Configure(ApplicationState.Intro)
            .Permit(ApplicationTrigger.ExitState, ApplicationState.Battle_First);

        _machine.Configure(ApplicationState.Battle_First)
            .Permit(ApplicationTrigger.GoToDefeat, ApplicationState.Defeat)
            .Permit(ApplicationTrigger.ExitState, ApplicationState.ExploreArea_AfterFirstBattle);

        _machine.Configure(ApplicationState.ExploreArea_AfterFirstBattle)
            .Permit(ApplicationTrigger.ExitState, ApplicationState.Battle_Final);

        _machine.Configure(ApplicationState.Battle_Final)
            .Permit(ApplicationTrigger.GoToDefeat, ApplicationState.Defeat)
            .Permit(ApplicationTrigger.ExitState, ApplicationState.Victory);

        _machine.Configure(ApplicationState.Defeat)
            .Permit(ApplicationTrigger.ExitState, ApplicationState.MainMenu);

        _machine.Configure(ApplicationState.Victory)
            .Permit(ApplicationTrigger.ExitState, ApplicationState.MainMenu);

        _machine.Configure(ApplicationState.SmurfManagement)
            .Permit(ApplicationTrigger.GoToHome, ApplicationState.MainMenu);

        _machine.Configure(ApplicationState.PeopleManagement)
            .Permit(ApplicationTrigger.GoToHome, ApplicationState.MainMenu);

        _machine.Configure(ApplicationState.ShuttingDown)
            .Ignore(ApplicationTrigger.ShutdownRequested);
    }

    public void Fire(
        ApplicationTrigger trigger)
    {
        if (_machine.CanFire(trigger))
        {
            var oldState = _state;
            _machine.Fire(trigger);

            if (oldState != _state)
            {
                RaiseStateChanged(oldState, _state);
            }
        }
        else
        {
            Console.WriteLine($"Ignored trigger {trigger} in state {_state}");
        }
    }

    private void RaiseStateChanged(
        ApplicationState oldState, 
        ApplicationState newState)
        => StateChanged?.Invoke(this, new ApplicationStateChangedEventArgs(oldState, newState));
}
