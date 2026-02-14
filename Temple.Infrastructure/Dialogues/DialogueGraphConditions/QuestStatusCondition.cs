using Temple.Application.DD;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class QuestStatusCondition : IDialogueGraphCondition
{
    public string QuestId { get; set; }
    public QuestStatus RequiredStatus { get; set; }

    public bool Evaluate(
        IDialogueQueryService query)
    {
        throw new NotImplementedException();
    }
}