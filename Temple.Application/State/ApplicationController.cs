namespace Temple.Application.State;

public class ApplicationController
{
    private readonly ApplicationStateMachine _stateMachine;

    public ApplicationController(ApplicationStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public ApplicationState CurrentState => _stateMachine.CurrentState;

    public event EventHandler<ApplicationStateChangedEventArgs> StateChanged
    {
        add => _stateMachine.StateChanged += value;
        remove => _stateMachine.StateChanged -= value;
    }

    public void Initialize()
    {
        _stateMachine.Fire(ApplicationTrigger.Initialize);
    }

    public void BeginWork()
    {
        _stateMachine.Fire(ApplicationTrigger.WorkRequested);
        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(2000); // simulate work
                _stateMachine.Fire(ApplicationTrigger.WorkCompleted);
            }
            catch
            {
                _stateMachine.Fire(ApplicationTrigger.ErrorOccurred);
            }
        });
    }

    public void Shutdown()
    {
        _stateMachine.Fire(ApplicationTrigger.ShutdownRequested);
    }
}
