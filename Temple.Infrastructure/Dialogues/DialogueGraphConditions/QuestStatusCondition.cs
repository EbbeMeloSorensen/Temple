using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class QuestStatusCondition : IDialogueGraphCondition
{
    public string QuestId { get; set; }
    public QuestStatus RequiredStatus { get; set; }

    public bool Evaluate(
        IDialogueQueryService query)
    {
        if (RequiredStatus.QuestState == QuestState.Hidden)
        {
            return query.IsQuestHidden(QuestId);
        }

        throw new NotImplementedException();
    }
}