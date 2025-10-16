using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities.Smurfs;
using Temple.Persistence.Repositories.Smurfs;

namespace Temple.Persistence.EFCore.AppData.Repositories.Smurfs
{
    public class SmurfRepository : Repository<Smurf>, ISmurfRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public SmurfRepository(DbContext context) : base(context)
        {
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.Smurfs);
            await Context.SaveChangesAsync();
        }

        public override Task Update(Smurf entity)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateRange(IEnumerable<Smurf> entities)
        {
            throw new NotImplementedException();
        }
    }
}
