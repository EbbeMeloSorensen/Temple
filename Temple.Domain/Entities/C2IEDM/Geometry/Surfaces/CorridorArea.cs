namespace Temple.Domain.Entities.C2IEDM.Geometry.Surfaces
{
    public class CorridorArea : Surface
    {
        public Guid CenterLineID { get; set; }
        public Line CenterLine { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public double WidthDimension { get; set; }
    }
}
