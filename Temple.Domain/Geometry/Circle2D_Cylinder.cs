using Craft.Math;

namespace Temple.Domain.Geometry
{
    public class Circle2D_Cylinder : Circle2D
    {
        public double Length { get; }

        public Circle2D_Cylinder(
            Point2D center,
            double radius,
            double length) : base(center, radius)
        {
            Length = length;
        }
    }
}
