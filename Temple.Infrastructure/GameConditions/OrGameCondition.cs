using Temple.Domain.Entities.DD.Common;

namespace Temple.Infrastructure.GameConditions;

public class OrGameCondition : IGameCondition
{
    public List<IGameCondition> Conditions { get; } = new();

    public bool Evaluate(
        IGameQueryService query)
    {
        return Conditions.Any(c => c.Evaluate(query));
    }
}