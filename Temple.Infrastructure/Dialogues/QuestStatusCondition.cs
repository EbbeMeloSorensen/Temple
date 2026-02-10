using Temple.Application.DD;

namespace Temple.Infrastructure.Dialogues;

public class QuestStatusCondition : IDialogueGraphCondition
{
    public string QuestId { get; set; }
    public QuestStatus RequiredStatus { get; set; }
}