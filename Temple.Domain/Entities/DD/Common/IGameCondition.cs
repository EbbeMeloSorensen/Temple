namespace Temple.Domain.Entities.DD.Common;

public interface IGameCondition
{
    bool Evaluate(
        IGameQueryService query);
}