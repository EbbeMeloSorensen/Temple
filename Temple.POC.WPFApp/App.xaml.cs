using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Smurfs;
using Temple.Infrastructure.Pagination;

namespace Temple.POC.WPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IHost _host;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var splash = new SplashScreenWindow();
            splash.Show();

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    //var connectionString = "Data source=babuska27.db";
                    //var connectionString = "Server=localhost;Port=5432;User Id=root;Password=root;Database=DB_DummyWpfApp";
                    var connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=L1on8Zebra;Database=DB_WPF_POC";

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

                    // Register our ViewModel and View
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddTransient<MainWindow>();
                })
                .Build();

            Task.Run(async () =>
            {
                using var scope = _host.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PRDbContextBase>();
                db.Database.MigrateAsync().GetAwaiter().GetResult();
                Seeding.SeedDatabase(db).GetAwaiter().GetResult();

                await Current.Dispatcher.InvokeAsync(() =>
                {
                    var scope = _host!.Services.CreateScope();
                    var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                    splash.Close();
                    mainWindow.Show();
                });
            });
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host is not null)
            {
                await _host.StopAsync();
            }

            _host?.Dispose();
            base.OnExit(e);
        }
    }
}
