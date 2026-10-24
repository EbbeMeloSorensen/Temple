namespace Temple.Application.State;

public enum ApplicationStateShiftTrigger
{
    Initialize,
    ShutdownRequested,
    ExitState,
    GoToSmurfManagement,
    GoToPeopleManagement,
    StartNewGame,
    LoadGame,
    GoToExploration,
    GoToInGameMenu,
    GoToDialogue,
    GoToBattle,
    GoToWilderness,
    GoToDefeat,
    GoToVictory,
}
