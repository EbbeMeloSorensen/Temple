using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public class ExclamationMark : ISiteComponent_Placeable
{
    public IGameCondition? Condition { get; set; }

    public Vector3D Position { get; set; }
}