using System;

namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public enum OrbitAreaAlignmentCode
    {
        Centre,
        Left,
        Right
    }

    public class OrbitArea : Surface
    {
        public Guid FirstPointID { get; set; }
        public Point FirstPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid SecondPointID { get; set; }
        public Point SecondPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public OrbitAreaAlignmentCode OrbitAreaAlignmentCode { get; set; }
        public double WidthDimension { get; set; }
    }
}