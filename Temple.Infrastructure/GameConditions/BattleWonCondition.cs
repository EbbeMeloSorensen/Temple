using Temple.Application.Interfaces;

namespace Temple.Infrastructure.GameConditions;

public class BattleWonCondition : IGameCondition
{
    public string BattleId { get; set; }

    public bool Evaluate(
        IGameQueryService query)
    {
        return query.IsBattleWon(BattleId);
    }
}