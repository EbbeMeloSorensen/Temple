using Temple.Domain.Entities.DD.Common;

namespace Temple.Infrastructure.GameConditions;

public class NotGameCondition : IGameCondition
{
    public IGameCondition Condition { get; set; }

    public bool Evaluate(
        IGameQueryService query)
    {
        return !Condition.Evaluate(query);
    }
}