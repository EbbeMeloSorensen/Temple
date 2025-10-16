using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities.C2IEDM.Geometry;

namespace Temple.Persistence.EFCore.AppData.EntityConfigurations.C2IEDM.Geometry.Locations.Line
{
    public class LinePointConfiguration : IEntityTypeConfiguration<LinePoint>
    {
        public void Configure(EntityTypeBuilder<LinePoint> builder)
        {
            builder.HasKey(lp => new { lp.LineID, lp.Index });
        }
    }
}
