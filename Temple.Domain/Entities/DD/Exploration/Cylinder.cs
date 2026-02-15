using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration
{
    public class Cylinder : ISiteComponent_Placeable
    {
        public IGameCondition? Condition { get; set; }

        public Vector3D Position { get; set; }

        public double Radius { get; set; }

        public double Length { get; set; }
    }
}
