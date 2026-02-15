using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public class Barrier : ISiteComponent
{
    public List<Vector3D> BarrierPoints { get; set; }

    public IEnumerable<Vector2D> BoundaryPoints => BarrierPoints.Select(_ => new Vector2D(_.Z, -_.X));
}

