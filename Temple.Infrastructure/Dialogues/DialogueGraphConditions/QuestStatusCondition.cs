using Temple.Application.DD;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class QuestStatusCondition : IDialogueGraphCondition
{
    public string QuestId { get; set; }
    public QuestStatus RequiredStatus { get; set; }

    public bool Evaluate(
        IDialogueQueryService query)
    {
        return query.DoesQuestStatusEqualRequiredValue(QuestId, RequiredStatus);
    }
}