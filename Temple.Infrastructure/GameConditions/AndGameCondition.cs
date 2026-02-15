using Temple.Domain.Entities.DD.Common;

namespace Temple.Infrastructure.GameConditions;

public class AndGameCondition : IGameCondition
{
    public List<IGameCondition> Conditions { get; set; }

    public bool Evaluate(
        IGameQueryService query)
    {
        return Conditions.All(c => c.Evaluate(query));
    }
}