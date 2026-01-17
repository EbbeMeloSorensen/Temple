using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests;

public sealed class QuestRuntime
{
    private readonly IReadOnlyList<Quest> _quests;
    private readonly EventBus _eventBus;

    public QuestRuntime(
        IEnumerable<Quest> quests,
        EventBus eventBus)
    {
        _quests = quests.ToList();
        _eventBus = eventBus;

        _eventBus.Subscribe<IGameEvent>(OnGameEvent);
    }

    private void OnGameEvent(IGameEvent e)
    {
        foreach (var quest in _quests)
        {
            var oldState = quest.State;

            quest.HandleEvent(e);

            if (quest.State != oldState)
            {
                _eventBus.Publish(
                    new QuestStateChangedEvent(
                        quest.Id,
                        oldState,
                        quest.State));
            }
        }
    }
}

