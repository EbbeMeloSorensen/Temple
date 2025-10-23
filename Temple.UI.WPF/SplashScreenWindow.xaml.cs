using System.Windows;
using Temple.ViewModel;

namespace Temple.UI.WPF
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow(
            SplashScreenViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
