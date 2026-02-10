using Temple.Persistence.Repositories.PR;
using Temple.Persistence.Repositories.Smurfs;

namespace Temple.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        ISmurfRepository Smurfs { get; }

        IPersonRepository People { get; }
        IPersonCommentRepository PersonComments { get; }
        IPersonAssociationRepository PersonAssociations { get; }

        void Clear();

        void Complete();
    }
}
