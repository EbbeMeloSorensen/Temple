namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class AbsolutePoint : Point
    {
        public double LatitudeCoordinate { get; set; }
        public double LongitudeCoordinate { get; set; }

        public Guid? VerticalDistanceId { get; set; }

        public virtual VerticalDistance? VerticalDistance { get; set; }
    }
}
