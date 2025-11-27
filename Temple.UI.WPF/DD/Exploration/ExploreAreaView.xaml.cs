using Craft.Simulation.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Temple.ViewModel.DD;

namespace Temple.UI.WPF.DD.Exploration
{
    /// <summary>
    /// Interaction logic for ExploreAreaView.xaml
    /// </summary>
    public partial class ExploreAreaView : UserControl
    {
        private ExploreAreaViewModel ViewModel { get { return DataContext as ExploreAreaViewModel; }}

        public ExploreAreaView()
        {
            InitializeComponent();
        }

        private void ExploreAreaView_OnKeyDown(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.UpArrow, KeyEventType.KeyPressed);
                    break;
                case Key.Down:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.DownArrow, KeyEventType.KeyPressed);
                    break;
                case Key.Left:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.LeftArrow, KeyEventType.KeyPressed);
                    break;
                case Key.Right:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.RightArrow, KeyEventType.KeyPressed);
                    break;
                case Key.Space:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.Space, KeyEventType.KeyPressed);
                    break;
            }
        }

        private void ExploreAreaView_OnKeyUp(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.UpArrow, KeyEventType.KeyReleased);
                    break;
                case Key.Down:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.DownArrow, KeyEventType.KeyReleased);
                    break;
                case Key.Left:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.LeftArrow, KeyEventType.KeyReleased);
                    break;
                case Key.Right:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.RightArrow, KeyEventType.KeyReleased);
                    break;
                case Key.Space:
                    ViewModel.Engine.HandleKeyEvent(KeyboardKey.Space, KeyEventType.KeyReleased);
                    break;
            }
        }
    }
}
