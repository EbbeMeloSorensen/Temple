using Temple.Application.Core;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.ViewModel.DD.Battle.BusinessLogic;

namespace Temple.ViewModel.DD.Quests;

// Denne klasse overvåger ændringer i quest-tilstande. Den er bindeled mellem quest-logikken og brugergrænsefladen.
// Den publicerer et event, der bruges i forbindelse med opdatering af brugergrænsefladen
public sealed class QuestStatusReadModel
{
    private readonly Dictionary<string, QuestStatus> _quests =
        new Dictionary<string, QuestStatus>();

    public event EventHandler<QuestStatusChangedEventArgs>? QuestStatusChanged;

    public QuestStatusReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<QuestStateChangedEvent>(HandleQuestStateChanged);
    }

    public bool IsQuestAvailable(
        string questId)
    {
        if (_quests.TryGetValue(questId, out QuestStatus status))
        {
            return status.State == QuestState.Available;
        }

        return false;
    }

    public bool IsQuestActive(
        string questId)
    {
        if (_quests.TryGetValue(questId, out QuestStatus status))
        {
            return status.State == QuestState.Active;
        }

        return false;
    }

    public bool IsQuestCompleted(
        string questId)
    {
        if (_quests.TryGetValue(questId, out QuestStatus status))
        {
            return status.State == QuestState.Completed;
        }

        return false;
    }

    private void HandleQuestStateChanged(
        QuestStateChangedEvent e)
    {
        if (!_quests.TryGetValue(e.QuestId, out var status))
        {
            status = new QuestStatus(e.QuestId, e.NewState);
            _quests.Add(e.QuestId, status);
        }
        else
        {
            status.UpdateState(e.NewState);
        }

        OnQuestStatusChanged(e.QuestId, e.NewState);
    }

    private void OnQuestStatusChanged(
        string questId,
        QuestState questState)
    {
        QuestStatusChanged?.Invoke(this, new QuestStatusChangedEventArgs(questId, questState));
    }
}