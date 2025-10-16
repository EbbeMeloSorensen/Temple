using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PR.Persistence.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityPersistence<TContext>(
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
