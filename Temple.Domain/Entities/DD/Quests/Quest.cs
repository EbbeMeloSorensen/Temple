using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Entities.DD.Quests.Rules;

namespace Temple.Domain.Entities.DD.Quests;

public sealed class Quest
{
    public string Id { get; }
    public QuestState State { get; private set; }
    public bool IsCompletionConditionMet { get; private set; }

    private readonly List<IQuestRule> _rules;

    public Quest(
        string id,
        IEnumerable<IQuestRule> rules)
    {
        Id = id;
        State = QuestState.Hidden;
        IsCompletionConditionMet = false;
        _rules = rules.ToList();
    }

    public void HandleEvent(IGameEvent e)
    {
        foreach (var rule in _rules)
        {
            rule.Apply(this, e);
        }
    }

    // Controlled state mutation
    public void TransitionTo(QuestState newState)
    {
        State = newState;
    }

    public void MarkObjectivesCompleted()
    {
        IsCompletionConditionMet = true;
    }
}
