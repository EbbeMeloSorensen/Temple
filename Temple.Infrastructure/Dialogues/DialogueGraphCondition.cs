using Temple.Application.DD;

namespace Temple.Infrastructure.Dialogues;

public class DialogueGraphCondition
{
    public string QuestId { get; set; }
    public QuestStatus RequiredStatus { get; set; }
}