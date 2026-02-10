namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class PointReference : CoordinateSystem
    {
        public Guid OriginPointID { get; set; }
        public Point OriginPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid XVectorPointID { get; set; }
        public Point XVectorPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid YVectorPointID { get; set; }
        public Point YVectorPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)
    }
}