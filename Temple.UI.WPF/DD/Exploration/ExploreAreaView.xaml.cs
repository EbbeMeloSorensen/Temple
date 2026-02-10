using Craft.Simulation.Engine;
using System.Windows.Controls;
using System.Windows.Input;
using Temple.ViewModel.DD.Exploration;

namespace Temple.UI.WPF.DD.Exploration
{
    /// <summary>
    /// Interaction logic for ExploreAreaView.xaml
    /// </summary>
    public partial class ExploreAreaView : UserControl
    {
        private ExplorationViewModel ViewModel { get { return DataContext as ExplorationViewModel; }}

        public ExploreAreaView()
        {
            InitializeComponent();

            // Er det her nødvendigt?
            this.PreviewLostKeyboardFocus += (s, e) =>
            {
                e.Handled = true; // Do not allow other controls to gain focus
            };
        }

        private void ExploreAreaView_OnKeyDown(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

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
            if (ViewModel == null)
            {
                return;
            }

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
