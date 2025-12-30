namespace Temple.Application.State
{
    // These are all the states of the state machine. They should be enums in order to work with Stateless.
    // Most of these correspond to a viewmodel

    public enum StateMachineState
    {
        Starting,
        MainMenu,
        ShuttingDown,
        SmurfManagement,
        PeopleManagement,
        Interlude,
        Exploration,
        Dialogue,
        Battle,
        Wilderness,
        Defeat,
        Victory
    }
}
