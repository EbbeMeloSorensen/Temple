using Temple.Application.DD;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.GameConditions;

public class QuestStatusCondition : IGameCondition
{
    public string QuestId { get; set; }
    public QuestStatus RequiredStatus { get; set; }

    public bool Evaluate(
        IGameQueryService query)
    {
        return query.DoesQuestStatusEqualRequiredValue(QuestId, RequiredStatus);
    }
}