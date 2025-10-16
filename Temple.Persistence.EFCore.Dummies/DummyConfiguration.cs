using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Temple.Persistence.EFCore.Dummies
{
    public class DummyConfiguration : IEntityTypeConfiguration<Dummy>
    {
        public void Configure(
            EntityTypeBuilder<Dummy> builder)
        {
            builder.HasKey(_ => _.ID);
        }
    }
}
