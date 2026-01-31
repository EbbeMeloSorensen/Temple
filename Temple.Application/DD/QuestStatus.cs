using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.DD;

public record QuestStatus
{
    public QuestState QuestState { get; set; }
    public bool AreCompletionCriteriaSatisfied { get; set; }
}