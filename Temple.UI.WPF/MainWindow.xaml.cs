using System.ComponentModel;
using System.Windows;
using Temple.ViewModel;

namespace Temple.UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow(
            MainWindowViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = viewModel;

            viewModel.ShutdownAction = () =>
            {
                _viewModel.ShutDownEngineIfRunning();
                System.Windows.Application.Current.Shutdown();
            };
        }

        private void MainWindow_OnClosing(
            object? sender,
            CancelEventArgs e)
        {
            _viewModel.ShutdownAction!();
        }
    }
}