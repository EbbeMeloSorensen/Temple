using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests;

// Denne klasse overvåger ændringer i quest-tilstande. Den er bindeled mellem quest-logikken og brugergrænsefladen.
public sealed class QuestStatusView
{
    private readonly Dictionary<string, QuestStatus> _quests =
        new Dictionary<string, QuestStatus>();

    public QuestStatusView(
        EventBus eventBus)
    {
        eventBus.Subscribe<QuestStateChangedEvent>(OnQuestStateChanged);
    }

    private void OnQuestStateChanged(
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

        UpdateUI(status);
    }

    private void UpdateUI(
        QuestStatus status)
    {
        switch (status.State)
        {
            case QuestState.Available:
                ShowAvailable(status.QuestId);
                break;

            case QuestState.Active:
                ShowActive(status.QuestId);
                break;

            case QuestState.Completed:
                ShowCompleted(status.QuestId);
                break;
        }
    }

    private void ShowAvailable(string questId)
        => Console.WriteLine($"Quest available: {questId}");

    private void ShowActive(string questId)
        => Console.WriteLine($"Quest active: {questId}");

    private void ShowCompleted(string questId)
        => Console.WriteLine($"Quest completed: {questId}");
}
