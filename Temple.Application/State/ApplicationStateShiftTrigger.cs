namespace Temple.Application.State;

public enum ApplicationStateShiftTrigger
{
    Initialize,
    ShutdownRequested,
    ExitState,
    GoToSmurfManagement,
    GoToPeopleManagement,
    StartNewGame,
    GoToDefeat,
    GoToVictory//,
    //EncounterEnemy,
    //TalkToNpc,
    //EndBattle,
    //EndDialog
}