using Craft.Logging;
using Craft.Persistence;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities.PR;
using Temple.Persistence.Repositories.PR;
using System.Linq.Expressions;

namespace Temple.Persistence.EFCore.AppData.Repositories.PR
{
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public PersonRepository(
            DbContext context) : base(context)
        {
        }

        public ILogger Logger { get; }

        public async Task<Person> Get(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.People.SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person does not exist");
                }

                return person;
            });
        }

        public Task<IEnumerable<Person>> GetAllVariants(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Correct(
            Person person)
        {
            throw new NotImplementedException();
        }

        public Task CorrectRange(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }

        public Task Erase(
            Person person)
        {
            throw new NotImplementedException();
        }

        public Task EraseRange(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes()
        {
            throw new NotImplementedException();
        }

        public async Task<Person> GetIncludingComments(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.People
                    .Include(p => p.Comments)
                    .SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person does not exist");
                }

                return person;
            });
        }

        public async Task<IEnumerable<Person>> FindIncludingComments(
            Expression<Func<Person, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                var people = PrDbContext.People
                    .Include(p => p.Comments)
                    .Where(predicate);

                return people;
            });
        }

        public async Task<IEnumerable<Person>> FindIncludingComments(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            return await Task.Run(() =>
            {
                var predicate = predicates.Aggregate((c, n) => c.And(n));

                var people = PrDbContext.People
                    .Include(p => p.Comments)
                    .Where(predicate);

                return people;
            });
        }

        public override async Task Update(
            Person person)
        {
            await Task.Run(() => { });
        }

        public override async Task UpdateRange(
            IEnumerable<Person> people)
        {
            await Task.Run(() => { });
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.People);
            await Context.SaveChangesAsync();
        }
    }
}
