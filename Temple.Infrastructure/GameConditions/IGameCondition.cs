using Temple.Application.Interfaces;

namespace Temple.Infrastructure.GameConditions;

public interface IGameCondition
{
    bool Evaluate(
        IGameQueryService query);
}