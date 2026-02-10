namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class SphereVolume : GeometricVolume
    {
        public Guid CentrePointID { get; set; }
        public Point CentrePoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public double RadiusDimension { get; set; }
    }
}
