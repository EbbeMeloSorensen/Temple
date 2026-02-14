namespace Temple.Application.Interfaces;

public interface IDialogueQueryService
{
    bool IsFactEstablished(
        string factID);

    bool IsQuestHidden(
        string questID);

    bool IsQuestAvailable(
        string questID);

    bool IsQuestReadyToTurnIn(
        string questID);

    bool IsQuestCompleted(
        string questID);

    bool IsBattleWon(
        string battleID);
}