using Craft.Math;
using Craft.Simulation.Boundaries;

namespace Temple.ViewModel.DD.Exploration.Boundaries;

public class NPC : CircularBoundary
{
    public NPC(Vector2D center, double radius, string tag = null) : base(center, radius, tag)
    {
    }
}

