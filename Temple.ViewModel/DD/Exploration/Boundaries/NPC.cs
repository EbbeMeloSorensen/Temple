using Craft.Math;
using Craft.Simulation.Boundaries;

namespace Temple.ViewModel.DD.Exploration.Boundaries;

public class NPC : CircularBoundary
{
    public string ModelId { get; }
    public double Orientation { get; }

    public NPC(
        Vector2D center,
        double radius,
        string tag,
        string modelId,
        double orientation) : base(center, radius, tag)
    {
        ModelId = modelId;
        Orientation = orientation;
    }
}

