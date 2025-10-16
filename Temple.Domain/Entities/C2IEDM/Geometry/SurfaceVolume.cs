using System;

namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class SurfaceVolume : GeometricVolume
    {
        public Guid DefiningSurfaceID { get; set; }
        public Surface DefiningSurface { get; set; } //= null!; (Forstyrrer Enterprise Architect)
    }
}
