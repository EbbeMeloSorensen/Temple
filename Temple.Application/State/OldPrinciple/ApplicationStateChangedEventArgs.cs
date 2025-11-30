namespace Temple.Application.State.OldPrinciple;

public class ApplicationStateChangedEventArgs : EventArgs
{
    public ApplicationState OldState { get; }
    public ApplicationState NewState { get; }

    public ApplicationStateChangedEventArgs(ApplicationState oldState, ApplicationState newState)
    {
        OldState = oldState;
        NewState = newState;
    }
}

