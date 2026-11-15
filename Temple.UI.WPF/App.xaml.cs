using Craft.UIElements.Dialogs;
using Craft.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Temple.Application.Core;
using Temple.ViewModel;

namespace Temple.UI.WPF
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
                        .ConfigureServices(services => services.AddWpfServices())
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
