using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Smurfs;
using Temple.Infrastructure.Pagination;

namespace Temple.POC.ConsoleApp
{
    internal class Program
    {
        static async Task Method1(string[] args)
        {
            //var host = Host.CreateDefaultBuilder(args)
            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    // Turn off logging entirely
                    //logging.ClearProviders();

                    // Filter EF Core logs
                    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
                    logging.AddFilter("Microsoft.EntityFrameworkCore.Migrations", LogLevel.None);
                })
                .ConfigureServices((context, services) =>
                {
                    // Load connection string from appsettings.json or environment
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    //connectionString = "Data source=babuska2.db";
                    connectionString = "Server=localhost;Port=5432;User Id=root;Password=root;Database=DB_DummyConsoleApp";

                    services.AddAppDataPersistence<PRDbContextBase>(options =>
                    {
                        //options.UseSqlite(connectionString);
                        options.UseNpgsql(connectionString);
                    });

                    services.AddAutoMapper(assemblies: typeof(MappingProfiles).Assembly);
                    services.AddApplication();   // registers MediatR and handlers
                    services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
                    services.AddScoped<IPagingHandler<SmurfDto>, PagingHandler<SmurfDto>>();

                    services.AddMediatR(cfg =>
                        cfg.RegisterServicesFromAssemblyContaining<List.Query>());
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PRDbContextBase>();
            await db.Database.MigrateAsync();
            await Seeding.SeedDatabase(db);

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new List.Query { Params = new SmurfParams() });

            Console.WriteLine($"\nSmurfs:");
            foreach (var smurf in result.Value)
            {
                Console.WriteLine($" {smurf.Name}");
            }
        }

        static async Task Method2(string[] args)
        {
            var services = new ServiceCollection();
            //var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
            var connectionString = "Data source=babuska2.db";

            services.AddAppDataPersistence<PRDbContextBase>(options =>
                options.UseSqlite(connectionString));

            services.AddAutoMapper(assemblies: typeof(MappingProfiles).Assembly);
            services.AddApplication();   // registers MediatR and handlers
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IPagingHandler<SmurfDto>, PagingHandler<SmurfDto>>();

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblyContaining<List.Query>());

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var result = await mediator.Send(new List.Query { Params = new SmurfParams() });

            Console.WriteLine($"\nSmurfs:");
            foreach (var smurf in result.Value)
            {
                Console.WriteLine($" {smurf.Name}");
            }
        }

        static async Task Main(string[] args)
        {
            // Method1 anvender en host, et scope og laver databasen, hvis den ikke er der
            await Method1(args);

            // Method 2 virker kun, hvis databasen er lavet
            //await Method2(args);

            Console.WriteLine("\nDone");
        }
    }
}
