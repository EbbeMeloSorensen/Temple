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

    // Rehydrate progress without executing rules or raising gameplay events.
    public void RestoreProgress(QuestState state, bool completionCriteriaSatisfied)
    {
        if (!Enum.IsDefined(state))
            throw new ArgumentOutOfRangeException(nameof(state));
        if (state == QuestState.Completed && !completionCriteriaSatisfied)
            throw new ArgumentException("A completed quest must have satisfied its completion criteria.");

        State = state;
        AreCompletionCriteriaSatisfied = completionCriteriaSatisfied;
    }
}
