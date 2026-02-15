using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration
{
    public class Cylinder : ISiteComponent_Placeable
    {
        public Vector3D Position { get; set; }
        public double Radius { get; set; }
        public double Length { get; set; }
    }
}
