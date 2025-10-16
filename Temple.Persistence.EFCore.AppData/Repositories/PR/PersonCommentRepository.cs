using Craft.Logging;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities.PR;
using Temple.Persistence.Repositories.PR;

namespace Temple.Persistence.EFCore.AppData.Repositories.PR
{
    public class PersonCommentRepository : Repository<PersonComment>, IPersonCommentRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public PersonCommentRepository(
            DbContext context) : base(context)
        {
        }

        public ILogger Logger { get; }

        public async Task<PersonComment> Get(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.PersonComments.SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person Comment does not exist");
                }

                return person;
            });
        }

        public Task Erase(PersonComment personComment)
        {
            throw new NotImplementedException();
        }

        public Task EraseRange(IEnumerable<PersonComment> personComments)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonComment>> GetAllVariants(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(
            PersonComment personComment)
        {
            await Task.Run(() => { });
        }

        public override async Task UpdateRange(
            IEnumerable<PersonComment> people)
        {
            await Task.Run(() => { });
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.PersonComments);
            await Context.SaveChangesAsync();
        }
    }
}
