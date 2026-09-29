using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Temple.Persistence.EFCore.AppData
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDataPersistence<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction)
            where TContext : PRDbContextBase
        {
            //services.AddDbContext<TContext>(optionsAction);

            services.AddDbContextFactory<TContext>(optionsAction);
            services.AddScoped<IAppDataInitializer, AppDataInitializer<TContext>>();

            // register repositories, unit of work, etc.
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
