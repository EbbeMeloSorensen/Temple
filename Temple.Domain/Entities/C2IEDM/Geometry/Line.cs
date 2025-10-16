using System.Collections.Generic;

namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class Line : Location
    {
        public virtual ICollection<LinePoint>? LinePoints { get; set; }
    }
}
