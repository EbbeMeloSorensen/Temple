using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration
{
    public class Sphere : ISiteComponent_Placeable
    {
        public IGameCondition? ConditionForPresence { get; set; }

        public Vector3D Position { get; set; }

        public double Radius { get; set; }
    }
}
