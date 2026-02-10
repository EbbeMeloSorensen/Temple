using Craft.Logging;

namespace Temple.Persistence.Versioned
{
    public class UnitOfWorkFactoryFacade : IUnitOfWorkFactoryVersioned, IUnitOfWorkFactoryHistorical
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DateTime? DatabaseTime { get; set; }
        public DateTime? HistoricalTime { get; set; }
        public bool IncludeCurrentObjects { get; set; }
        public bool IncludeHistoricalObjects { get; set; }

        public ILogger Logger { get; set; }

        public UnitOfWorkFactoryFacade(
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            IncludeCurrentObjects = true;
            Logger = _unitOfWorkFactory.Logger;
        }

        public void Initialize(
            bool versioned)
        {
            _unitOfWorkFactory.Initialize(versioned);
        }

        public void OverrideConnectionString(
            string connectionString)
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWorkFacade(
                Logger,
                _unitOfWorkFactory.GenerateUnitOfWork(),
                HistoricalTime,
                DatabaseTime,
                IncludeCurrentObjects,
                IncludeHistoricalObjects);
        }

        public void Reseed()
        {
            _unitOfWorkFactory.Reseed();
        }
    }
}
