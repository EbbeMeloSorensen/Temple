using Craft.Math;
using Craft.Simulation.Boundaries;

namespace Temple.ViewModel.DD.Exploration.Boundaries
{
    public class Cylinder : CircularBoundary
    {
        public double Length { get; }

        public Cylinder(
            Vector2D center,
            double radius,
            string tag,
            double length) : base(center, radius, tag)
        {
            Length = length;
        }
    }
}
