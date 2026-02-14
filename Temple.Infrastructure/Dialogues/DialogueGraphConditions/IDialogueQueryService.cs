namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public interface IDialogueQueryService
{
    bool IsFactEstablished(
        string factID);

    bool IsQuestCompleted(
        string questID);

    bool IsBattleWon(
        string battleID);
}