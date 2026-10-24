using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities.Smurfs;

namespace Temple.Persistence.EFCore.AppData.EntityConfigurations.Smurfs;

public class SmurfConfiguration : IEntityTypeConfiguration<Smurf>
{
    public void Configure(EntityTypeBuilder<Smurf> builder)
    {
        builder.Property(smurf => smurf.Role)
            .HasDefaultValue(SmurfRole.Unspecified);
    }
}
