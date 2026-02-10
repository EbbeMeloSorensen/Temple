namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class RelativePoint : Point
    {
        public Guid CoordinateSystemID { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public double XCoordinateDimension { get; set; }
        public double YCoordinateDimension { get; set; }
        public double ZCoordinateDimension { get; set; }
    }
}
