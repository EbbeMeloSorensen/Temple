namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class TrackArea : Surface
    {
        public Guid BeginPointID { get; set; }
        public Point BeginPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid EndPointID { get; set; }
        public Point EndPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public double LeftWidthDimension { get; set; }
        public double RightWidthDimension { get; set; }
    }
}
