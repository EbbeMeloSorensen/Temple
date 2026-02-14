using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Application.Interfaces.Readers;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class DialogueQueryService : IDialogueQueryService
{
    private IFactsEstablishedReader _factsEstablishedReader;
    private IQuestStatusReader _questStatusReader;
    private IBattlesWonReader _battlesWonReader;
    private ISitesUnlockedReader _sitesUnlockedReader;

    public DialogueQueryService(
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