using Temple.Application.DD;

namespace Temple.Application.Interfaces;

public interface IDialogueQueryService
{
    bool IsFactEstablished(
        string factId);

    bool DoesQuestStatusEqualRequiredValue(
        string questId,
        QuestStatus status);

    bool IsBattleWon(
        string battleId);
}