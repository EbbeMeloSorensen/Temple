using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public class Barrier : ISiteComponent
{
    public IGameCondition? ConditionForPresence { get; set; }

    public List<Vector2D> BarrierPoints { get; set; }
}

