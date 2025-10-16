using System;

namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class PolyArcArea : Surface
    {
        public Guid DefiningLineID { get; set; }
        public Line DefiningLine { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid BearingOriginPointID { get; set; }
        public Point BearingOriginPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public double BeginBearingAngle { get; set; }
        public double EndBearingAngle { get; set; }
        public double ArcRadiusDimension { get; set; }
    }
}
