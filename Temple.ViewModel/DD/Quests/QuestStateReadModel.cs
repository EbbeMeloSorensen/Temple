using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.ViewModel.DD.Quests;

// Denne klasse overvåger ændringer i quest-tilstande. Den er bindeled mellem quest-logikken og brugergrænsefladen.
// Den publicerer et event, der bruges i forbindelse med opdatering af brugergrænsefladen
public sealed class QuestStateReadModel : IQuestStateReadModel
{
    private readonly Dictionary<string, QuestState> _quests =
        new Dictionary<string, QuestState>();

    public event EventHandler<QuestStateChangedEventArgs>? QuestStateChanged;

    public QuestStateReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<QuestStateChangedEvent>(HandleQuestStateChanged);
    }

    public QuestState GetQuestState(
        string questId)
    {
        return _quests.TryGetValue(questId, out QuestState state) ? state : QuestState.Hidden;
    }

    private void HandleQuestStateChanged(
        QuestStateChangedEvent e)
    {
        if (!_quests.TryGetValue(e.QuestId, out var status))
        {
            _quests.Add(e.QuestId, e.NewState);
        }
        else
        {
            _quests[e.QuestId] = e.NewState;
        }

        OnQuestStateChanged(e.QuestId, e.NewState);
    }

    private void OnQuestStateChanged(
        string questId,
        QuestState questState)
    {
        QuestStateChanged?.Invoke(this, new QuestStateChangedEventArgs(questId, questState));
    }
}