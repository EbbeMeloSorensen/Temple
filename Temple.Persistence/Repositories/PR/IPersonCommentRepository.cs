using Craft.Logging;
using Craft.Persistence;
using Temple.Domain.Entities.PR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Temple.Persistence.Repositories.PR
{
    public interface IPersonCommentRepository : IRepository<PersonComment>
    {
        ILogger Logger { get; }

        Task<PersonComment> Get(
            Guid id);

        Task Erase(
            PersonComment personComment);

        Task EraseRange(
            IEnumerable<PersonComment> personComments);
    }
}
