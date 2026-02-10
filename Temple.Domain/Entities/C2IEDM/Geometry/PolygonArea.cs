namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class PolygonArea : Surface
    {
        public Guid BoundingLineID { get; set; }
        public Line BoundingLine { get; set; } //= null!; (Forstyrrer Enterprise Architect)
    }
}
