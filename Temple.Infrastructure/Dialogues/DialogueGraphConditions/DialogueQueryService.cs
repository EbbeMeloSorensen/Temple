using Temple.Application.Interfaces;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class DialogueQueryService : IDialogueQueryService
{
    private IQuestStatusReader _questStatusReader;

    public DialogueQueryService(
        IQuestStatusReader questStatusReader)
    {
        _questStatusReader = questStatusReader;
    }

    public bool IsFactEstablished(
        string factID)
    {
        throw new NotImplementedException();
    }

    public bool IsQuestHidden(
        string questId)
    {
        return _questStatusReader.GetQuestStatus(questId).QuestState == QuestState.Hidden;
    }

    public bool IsQuestAvailable(
        string questID)
    {
        throw new NotImplementedException();
    }

    public bool IsQuestReadyToTurnIn(
        string questID)
    {
        throw new NotImplementedException();
    }

    public bool IsQuestCompleted(
        string questID)
    {
        throw new NotImplementedException();
    }

    public bool IsBattleWon(
        string battleID)
    {
        throw new NotImplementedException();
    }
}