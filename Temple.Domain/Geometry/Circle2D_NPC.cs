using Craft.Math;

namespace Temple.Domain.Geometry;

public class Circle2D_NPC : Circle2D
{
    public string ModelId { get; }
    public double Orientation { get; }

    public Circle2D_NPC(
        Point2D center,
        double radius,
        string modelId,
        double orientation) : base(center, radius)
    {
        ModelId = modelId;
        Orientation = orientation;
    }
}
