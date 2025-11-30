using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Smurfs;
using Temple.Application.State.NewPrinciple;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.Infrastructure.Pagination;
using Temple.Application.State.OldPrinciple;

namespace Temple.POC.State.WPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IHost _host;

        protected override async void OnStartup(
            StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var splashScreenViewModel = new SplashScreenViewModel();
                var splashScreen = new SplashScreenWindow(splashScreenViewModel);
                splashScreen.Show();

                _host = await Task.Run(() =>
                {
                    return Host.CreateDefaultBuilder()
                        .ConfigureServices((context, services) =>
                        {
                            //var connectionString = "Data source=babuska27.db";
                            //var connectionString = "Server=localhost;Port=5432;User Id=root;Password=root;Database=DB_DummyWpfApp";
                            var connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=L1on8Zebra;Database=DB_WPF_POC_5";

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

                            // Register stuff for our state machine
                            services.AddSingleton<ApplicationStateMachine>();
                            services.AddSingleton<GameStateMachine>();
                            services.AddSingleton<ApplicationController>();
                        })
                        .Build();
                });

                await _host.StartAsync();

                var controller = _host.Services.GetRequiredService<ApplicationController>();

                controller.ProgressChanged += (s, msg) =>
                {
                    // Marshal to UI thread
                    Dispatcher.Invoke(() => splashScreenViewModel.StatusMessage = msg);
                };

                await controller.InitializeAsync();

                await Current.Dispatcher.InvokeAsync(() =>
                {
                    var scope = _host!.Services.CreateScope();
                    var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
                    splashScreen.Close();
                    mainWindow.Show();
                });
            }
            catch (Exception exception)
            {
                // Handle or log gracefully
                MessageBox.Show($"Startup failed: {exception}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
            }
        }

        protected override async void OnExit(
            ExitEventArgs e)
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
