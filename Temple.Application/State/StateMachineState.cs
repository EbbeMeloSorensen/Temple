namespace Temple.Application.State
{
    // These are all the states of the state machine. They should be enums in order to work with Stateless.

    public enum StateMachineState
    {
        Starting,
        MainMenu,
        ShuttingDown,
        SmurfManagement,
        PeopleManagement,
        Interlude,
        Defeat,
        Victory,
        Battle,
        Exploration
    }
}
