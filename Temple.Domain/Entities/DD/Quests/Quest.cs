using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Entities.DD.Quests.Rules;

namespace Temple.Domain.Entities.DD.Quests;

public sealed class Quest
{
    public string Id { get; }
    public QuestState State { get; private set; }
    public bool AreCompletionCriteriaSatisfied { get; private set; }

    public List<IQuestRule> Rules { get; set; }

    public Quest(
        string id,
        IEnumerable<IQuestRule> rules)
    {
        Id = id;
        State = QuestState.Hidden;
        AreCompletionCriteriaSatisfied = false;
        Rules = rules.ToList();
    }

    public void HandleEvent(IGameEvent e)
    {
        foreach (var rule in Rules)
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
        AreCompletionCriteriaSatisfied = true;
    }
}
