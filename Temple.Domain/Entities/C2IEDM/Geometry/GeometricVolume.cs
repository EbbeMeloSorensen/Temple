using System;

namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public abstract class GeometricVolume : Location
    {
        public Guid? LowerVerticalDistanceID { get; set; }
        public virtual VerticalDistance? LowerVerticalDistance { get; set; }

        public Guid? UpperVerticalDistanceID { get; set; }
        public virtual VerticalDistance? UpperVerticalDistance { get; set; }
    }
}
