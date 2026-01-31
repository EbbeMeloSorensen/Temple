using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Application.Core;

// Dette er en klasse, som kun opererer i kraft af sine sideeffekter, som  er at lade alle quests i en collection
// subscribe til events fra en event bus. Derudover lytter den på events fra event bus'en og lader hver quest håndtere eventet.
public sealed class QuestRuntime
{
    private readonly IReadOnlyList<Quest> _quests;
    private readonly QuestEventBus _eventBus;

    public QuestRuntime(
        IEnumerable<Quest> quests,
        QuestEventBus eventBus)
    {
        _quests = quests.ToList();
        _eventBus = eventBus;

        _eventBus.Subscribe<IGameEvent>(OnGameEvent);
    }

    private void OnGameEvent(IGameEvent e)
    {
        // Der er sket noget, så dribl alle quests igennem
        foreach (var quest in _quests)
        {
            var stateBefore = quest.State;
            var completionCriteriaSatisfiedBefore = quest.AreCompletionCriteriaSatisfied;

            // Få hver quest til at håndtere eventet. Dette kan få nogle af dem til at skifte state
            quest.HandleEvent(e);

            if (quest.State != stateBefore)
            {
                // Informer subscribers, f.eks. readmodeller, om quests, der har skiftet state
                _eventBus.Publish(
                    new QuestStateChangedEvent(
                        quest.Id,
                        stateBefore,
                        quest.State));
            }

            // Informer subscribers, f.eks. readmodeller, om quests, hvor det, om deres acceptkriterier er opfyldt, har ændret sig
            if (quest.AreCompletionCriteriaSatisfied != completionCriteriaSatisfiedBefore)
            {
                _eventBus.Publish(
                    new QuestSatisfactionOfCompletionCriteriaChangedEvent(
                        quest.Id,
                        quest.AreCompletionCriteriaSatisfied));
            }
        }
    }
}