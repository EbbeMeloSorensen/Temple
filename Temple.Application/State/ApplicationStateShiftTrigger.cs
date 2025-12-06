namespace Temple.Application.State;

public enum ApplicationStateShiftTrigger
{
    Initialize,
    ShutdownRequested,
    ExitState,
    GoToSmurfManagement,
    GoToPeopleManagement,
    StartNewGame,
    GoToExploration,
    GoToBattle,
    GoToDefeat,
    GoToVictory,
}