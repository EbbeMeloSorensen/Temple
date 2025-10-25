using System.Windows;
using Temple.ViewModel;

namespace Temple.UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            viewModel.ShutdownAction = System.Windows.Application.Current.Shutdown;
        }
    }
}