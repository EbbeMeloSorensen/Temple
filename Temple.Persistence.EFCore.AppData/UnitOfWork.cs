using Temple.Persistence.Repositories.PR;
using Temple.Persistence.Repositories.Smurfs;
using Temple.Persistence.EFCore.AppData.Repositories.PR;
using Temple.Persistence.EFCore.AppData.Repositories.Smurfs;

namespace Temple.Persistence.EFCore.AppData
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PRDbContextBase _context;

        public ISmurfRepository Smurfs { get; }

        public IPersonRepository People { get; }
        public IPersonCommentRepository PersonComments { get; }
        public IPersonAssociationRepository PersonAssociations { get; }

        public UnitOfWork(
            PRDbContextBase context)
        {
            _context = context;

            Smurfs = new SmurfRepository(_context);

            People = new PersonRepository(_context);
            PersonComments = new PersonCommentRepository(_context);
            PersonAssociations = new PersonAssociationRepository(_context);
        }

        public void Clear()
        {
            Smurfs.Clear();

            //PersonAssociations.Clear();
            PersonComments.Clear();
            People.Clear();
        }

        public void Complete()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
