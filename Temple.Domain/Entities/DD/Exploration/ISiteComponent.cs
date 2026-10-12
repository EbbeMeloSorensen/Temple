using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public interface ISiteComponent
{
    public IGameCondition? ConditionForPresence { get; set; }
}

