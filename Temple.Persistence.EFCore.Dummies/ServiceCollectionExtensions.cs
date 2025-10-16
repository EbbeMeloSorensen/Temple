using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Temple.Persistence.EFCore.Dummies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDummyPersistence<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(optionsAction);

            // register repositories, unit of work, etc.
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
