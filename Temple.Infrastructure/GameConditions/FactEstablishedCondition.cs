using Temple.Domain.Entities.DD.Common;

namespace Temple.Infrastructure.GameConditions;

public class FactEstablishedCondition : IGameCondition
{
    public string FactId { get; set; }

    public bool Evaluate(
        IGameQueryService query)
    {
        return query.IsFactEstablished(FactId);
    }
}