using Craft.Logging;
using Microsoft.EntityFrameworkCore;

namespace Temple.Persistence.EFCore.AppData
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IDbContextFactory<PRDbContextBase> _dbContextFactory;

        public ILogger Logger { get; set; }

        public UnitOfWorkFactory(IDbContextFactory<PRDbContextBase> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Initialize(
            bool versioned)
        {
            throw new NotImplementedException();
            // PRDbContextBase.Versioned = versioned;
            // using var context = new PRDbContext();
            // context.Database.EnsureCreated();
            // Seeding.SeedDatabase(context);
        }

        public void OverrideConnectionString(
            string connectionString)
        {
            throw new NotImplementedException();
            //ConnectionStringProvider.ConnectionString = connectionString;
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            var context = _dbContextFactory.CreateDbContext();
            return new UnitOfWork(context);
            //throw new NotImplementedException();
            //return new UnitOfWork(new PRDbContext());
        }

        public void Reseed()
        {
            throw new NotImplementedException();
            // using var context = new PRDbContext();
            // context.Database.EnsureCreated();

            // using var unitOfWork = GenerateUnitOfWork();
            // unitOfWork.Clear();
            // unitOfWork.Complete();

            // Seeding.SeedDatabase(context);
            // unitOfWork.Complete();
        }
    }
}