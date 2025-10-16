using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities.PR;

namespace Temple.Persistence.EFCore.AppData.EntityConfigurations.PR
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        private bool _versioned;

        public PersonConfiguration(
            bool versioned)
        {
            _versioned = versioned;
        }

        public void Configure(
            EntityTypeBuilder<Person> builder)
        {
            if (_versioned)
            {
                builder.HasKey(p => p.ArchiveID);
            }
            else
            {
                builder.HasKey(p => p.ID);
            }
        }
    }
}
