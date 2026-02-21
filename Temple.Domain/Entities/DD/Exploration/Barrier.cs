using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public class Barrier : ISiteComponent
{
    public IGameCondition? Condition { get; set; }

    public List<Vector3D> BarrierPoints { get; set; }
}

