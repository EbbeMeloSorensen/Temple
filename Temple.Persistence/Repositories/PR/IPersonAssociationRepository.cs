using Craft.Logging;
using Craft.Persistence;
using Temple.Domain.Entities.PR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Temple.Persistence.Repositories.PR
{
    public interface IPersonAssociationRepository : IRepository<PersonAssociation>
    {
        ILogger Logger { get; }

        Task<PersonAssociation> Get(
            Guid id);

        Task<IEnumerable<PersonAssociation>> GetAllVariants(
            Guid id);
    }
}