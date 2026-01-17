using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests;

// Dette er en klasse, som kun opererer i kraft af sine sideeffekter, som  er at lade alle quests i en collection
// subscribe til events fra en event bus. Derudover lytter den på events fra event bus'en og lader hver quest håndtere eventet.
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

