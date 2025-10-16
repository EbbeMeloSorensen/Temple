using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Temple.Domain.Entities.PR;

namespace Temple.Persistence.EFCore.AppData.EntityConfigurations.PR
{
    public class PersonCommentConfiguration : IEntityTypeConfiguration<PersonComment>
    {
        private bool _versioned;

        public PersonCommentConfiguration(
            bool versioned)
        {
            _versioned = versioned;
        }

        public void Configure(
            EntityTypeBuilder<PersonComment> builder)
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
