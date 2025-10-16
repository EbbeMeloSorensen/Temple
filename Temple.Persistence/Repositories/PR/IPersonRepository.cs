using Craft.Logging;
using Craft.Persistence;
using Temple.Domain.Entities.PR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Temple.Persistence.Repositories.PR
{
    public interface IPersonRepository : IRepository<Person>
    {
        ILogger Logger { get; }

        Task<Person> Get(
            Guid id);

        Task<Person> GetIncludingComments(
            Guid id);

        Task<IEnumerable<Person>> FindIncludingComments(
            Expression<Func<Person, bool>> predicate);

        Task<IEnumerable<Person>> FindIncludingComments(
            IList<Expression<Func<Person, bool>>> predicates);

        Task<IEnumerable<Person>> GetAllVariants(
            Guid id);

        Task Correct(
            Person person);

        Task CorrectRange(
            IEnumerable<Person> people);

        Task Erase(
            Person person);

        Task EraseRange(
            IEnumerable<Person> people);

        Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema();

        Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes();
    }
}
