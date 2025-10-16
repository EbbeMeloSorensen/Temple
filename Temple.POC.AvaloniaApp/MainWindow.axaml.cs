using Avalonia.Controls;
using Temple.POC.AvaloniaApp.ViewModels;

namespace Temple.POC.AvaloniaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(); // simple, direct binding
        }
    }
}