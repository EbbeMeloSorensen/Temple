using Temple.Domain.Entities.DD.Common;

namespace Temple.Infrastructure.GameConditions;

public class KnowledgeGainedCondition : IGameCondition
{
    public string KnowledgeId { get; set; }

    public bool Evaluate(
        IGameQueryService query)
    {
        return query.IsKnowledgeGained(KnowledgeId);
    }
}