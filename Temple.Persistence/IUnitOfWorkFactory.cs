using Craft.Logging;

namespace Temple.Persistence
{
    public interface IUnitOfWorkFactory
    {
        ILogger Logger { get; set; }

        void Initialize(
            bool versioned);

        void OverrideConnectionString(
            string connectionString);

        IUnitOfWork GenerateUnitOfWork();

        void Reseed();
    }
}
