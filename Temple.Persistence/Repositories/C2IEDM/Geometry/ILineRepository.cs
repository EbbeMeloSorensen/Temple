using Craft.Persistence;
using Temple.Domain.Entities.C2IEDM.Geometry;
using System.Collections.Generic;

namespace Temple.Persistence.Repositories.C2IEDM.Geometry
{
    public interface ILineRepository : IRepository<Line>
    {
        IList<Line> GetLinesIncludingPoints();
    }
}
