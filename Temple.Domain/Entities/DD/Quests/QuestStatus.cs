namespace Temple.Domain.Entities.DD.Quests;

public record QuestStatus
{
    public QuestState QuestState { get; set; }
    public bool AreCompletionCriteriaSatisfied { get; set; }
}