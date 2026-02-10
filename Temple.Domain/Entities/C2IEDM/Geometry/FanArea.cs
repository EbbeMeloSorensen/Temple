namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class FanArea : Surface
    {
        public Guid VertexPointID { get; set; }
        public Point VertexPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public double MinimumRangeDimension { get; set; }
        public double MaximumRangeDimension { get; set; }
        public double OrientationAngle { get; set; }
        public double SectorSizeAngle { get; set; }
    }
}
