using Temple.Application.Interfaces;

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