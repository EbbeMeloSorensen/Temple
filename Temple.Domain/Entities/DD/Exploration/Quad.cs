using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public class Quad : ISiteComponent
{
    public IGameCondition? Condition { get; set; }


    public Vector3D Point1 { get; set; }

    public Vector3D Point2 { get; set; }

    public Vector3D Point3 { get; set; }

    public Vector3D Point4 { get; set; }
}

