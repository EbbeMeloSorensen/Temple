using Craft.Math;
using Craft.Simulation.Boundaries;

namespace Temple.ViewModel.DD.Exploration.Boundaries
{
    public class Trigger : LineSegment
    {
        public Trigger(
            Vector2D point1,
            Vector2D point2,
            string tag = null) : base(point1, point2, tag)
        {
        }
    }
}
