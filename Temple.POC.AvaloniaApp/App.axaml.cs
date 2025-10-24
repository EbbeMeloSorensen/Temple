using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Smurfs;
using Temple.Infrastructure.Pagination;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.POC.AvaloniaApp.ViewModels;

namespace Temple.POC.AvaloniaApp
{
    public partial class App : Avalonia.Application
    {
        private IHost? _host;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    //var connectionString = "Data source=babuska27.db";
                    //var connectionString = "Server=localhost;Port=5432;User Id=root;Password=root;Database=DB_DummyWpfApp";

                    // Postgres - MELO - Home
                    //var connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=L1on8Zebra;Database=DB_Avalonia";

                    // Postgres - MELO - Basement
                    var connectionString = "Server=localhost;Port=5432;User Id=root;Password=root;Database=DB_Temple_UI_Console";

                    // Postgres - Linux in podman
                    //var connectionString = "Server=localhost;Port=5432;User Id=myuser;Password=mypassword;Database=DB_Temple_Avalonia";

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

                    // Register the viewmodel
                    services.AddTransient<MainWindowViewModel>();
                })
                .Build();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var vm = _host.Services.GetRequiredService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = vm
                };

                // Show the GUI first
                desktop.MainWindow.Opened += async (_, _) =>
                {
                    try
                    {
                        await MigrateDatabaseAsync(_host.Services);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Database migration failed: {ex}");
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static async Task MigrateDatabaseAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PRDbContextBase>();
            await db.Database.MigrateAsync();
            await Seeding.SeedDatabase(db);
        }
    }
}