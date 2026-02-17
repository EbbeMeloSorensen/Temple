using Temple.Domain.Entities.DD.Quests;

namespace Temple.Domain.Entities.DD.Common;

public interface IGameQueryService
{
    bool IsKnowledgeGained(
        string knowledgeId);

    bool IsFactEstablished(
        string factId);

    bool DoesQuestStatusEqualRequiredValue(
        string questId,
        QuestStatus status);

    bool IsBattleWon(
        string battleId);
}