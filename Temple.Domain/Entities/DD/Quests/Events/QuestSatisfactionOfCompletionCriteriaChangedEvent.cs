namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class QuestSatisfactionOfCompletionCriteriaChangedEvent : IGameEvent
{
    public string QuestId { get; }
    public bool AreCompletionCriteriaSatisfied { get; }

    public QuestSatisfactionOfCompletionCriteriaChangedEvent(
        string questId,
        bool areCompletionCriteriaSatisfied)
    {
        QuestId = questId;
        AreCompletionCriteriaSatisfied = areCompletionCriteriaSatisfied;
    }
}