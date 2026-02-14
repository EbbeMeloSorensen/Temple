using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public class Quad : SiteComponent
{
    public Vector3D Point1 { get; set; }
    public Vector3D Point2 { get; set; }
    public Vector3D Point3 { get; set; }
    public Vector3D Point4 { get; set; }
}

