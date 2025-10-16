using Craft.Logging;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities.PR;
using Temple.Persistence.Repositories.PR;

namespace Temple.Persistence.EFCore.AppData.Repositories.PR
{
    public class PersonAssociationRepository : Repository<PersonAssociation>, IPersonAssociationRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public PersonAssociationRepository(
            DbContext context) : base(context)
        {
        }

        public ILogger Logger { get; }

        public async Task<PersonAssociation> Get(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.PersonAssociations.SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person Association does not exist");
                }

                return person;
            });
        }

        public Task<IEnumerable<PersonAssociation>> GetAllVariants(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(
            PersonAssociation personAssociation)
        {
            await Task.Run(() => { });
        }

        public override async Task UpdateRange(
            IEnumerable<PersonAssociation> personAssociations)
        {
            await Task.Run(() => { });
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.PersonAssociations);
            await Context.SaveChangesAsync();
        }
    }
}