using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Quests;
using Temple.Application.Interfaces.Readers;

namespace Temple.Infrastructure.GameConditions;

public class GameQueryService : IGameQueryService
{
    private IFactsEstablishedReader _factsEstablishedReader;
    private IQuestStatusReader _questStatusReader;
    private IBattlesWonReader _battlesWonReader;
    private ISitesUnlockedReader _sitesUnlockedReader;

    public GameQueryService(
        IFactsEstablishedReader factsEstablishedReader,
        IQuestStatusReader questStatusReader,
        IBattlesWonReader battlesWonReader,
        ISitesUnlockedReader sitesUnlockedReader)
    {
        _factsEstablishedReader = factsEstablishedReader;
        _questStatusReader = questStatusReader;
        _battlesWonReader = battlesWonReader;
        _sitesUnlockedReader = sitesUnlockedReader;
    }

    public bool IsFactEstablished(
        string factID)
    {
        return _factsEstablishedReader.FactEstablished(factID);
    }

    public bool DoesQuestStatusEqualRequiredValue(
        string questId,
        QuestStatus status)
    {
        return _questStatusReader.GetQuestStatus(questId) == status;
    }

    public bool IsBattleWon(
        string battleID)
    {
        return _battlesWonReader.BattleWon(battleID);
    }
}