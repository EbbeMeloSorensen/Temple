using Craft.Logging;
using Temple.Persistence.Repositories.PR;
using Temple.Persistence.Repositories.Smurfs;
using System;
using Temple.Persistence.Versioned.Repositories;

namespace Temple.Persistence.Versioned
{
    public class UnitOfWorkFacade : IUnitOfWork
    {
        private DateTime? _transactionTime;

        internal IUnitOfWork UnitOfWork { get; }
        internal DateTime? DatabaseTime { get; }
        internal DateTime? HistoricalTime { get; }
        internal bool IncludeCurrentObjects { get; }
        internal bool IncludeHistoricalObjects { get; }

        // When TransactionTime is accessed, it will be set to the current UTC time if it hasn't been set already.
        internal DateTime TransactionTime => _transactionTime ??= DateTime.UtcNow;

        public DateTime? TimeOfChange { get; set; }

        public ISmurfRepository Smurfs { get; }

        public IPersonRepository People { get; }
        public IPersonCommentRepository PersonComments { get; }
        public IPersonAssociationRepository PersonAssociations { get; }

        public UnitOfWorkFacade(
            ILogger logger,
            IUnitOfWork unitOfWork,
            DateTime? historicalTime,
            DateTime? databaseTime,
            bool includeCurrentObjects,
            bool includeHistoricalObjects)
        {
            UnitOfWork = unitOfWork;
            HistoricalTime = historicalTime;
            DatabaseTime = databaseTime;
            IncludeCurrentObjects = includeCurrentObjects;
            IncludeHistoricalObjects = includeHistoricalObjects;

            People = new PersonRepositoryFacade(logger, this);
            PersonComments = new PersonCommentRepositoryFacade(logger, this);
        }

        public void Clear()
        {
            UnitOfWork.Clear();
        }

        public void Complete()
        {
            UnitOfWork.Complete();
            _transactionTime = null;
            TimeOfChange = null;
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}
