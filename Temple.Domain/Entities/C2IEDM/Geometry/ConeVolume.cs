using System;

namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class ConeVolume : GeometricVolume
    {
        public Guid DefiningSurfaceID { get; set; }
        public Surface DefiningSurface { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid VertexPointID { get; set; }
        public Point VertexPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)
    }
}
