using Craft.Simulation;
using Craft.Simulation.Engine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Temple.ViewModel.DD.Exploration;

namespace Temple.UI.WPF.DD.Exploration
{
    public partial class ExploreAreaView : UserControl
    {
        private ExplorationViewModel? _inputViewModel;
        private Window? _window;
        private ExplorationViewModel? ViewModel => DataContext as ExplorationViewModel;

        public ExploreAreaView()
        {
            InitializeComponent();
            Loaded += (_, _) => AttachInput();
            Unloaded += (_, _) => DetachInput();
            DataContextChanged += (_, _) =>
            {
                DetachInput();
                if (IsLoaded) AttachInput();
            };
            PreviewLostKeyboardFocus += (s, e) =>
            {
                e.Handled = true; // Preserve the exploration view's focus policy.
            };
        }

        private void AttachInput()
        {
            DetachInput();
            _inputViewModel = ViewModel;
            if (_inputViewModel != null) _inputViewModel.CaptureKeyboard = CaptureKeyboard;
            _window = Window.GetWindow(this);
            if (_window != null) _window.Deactivated += Window_Deactivated;
        }

        private void DetachInput()
        {
            if (_window != null) _window.Deactivated -= Window_Deactivated;
            _window = null;
            if (_inputViewModel != null)
            {
                _inputViewModel.CaptureKeyboard = () => new KeyboardState();
                _inputViewModel.KeyPresses.Clear();
                _inputViewModel = null;
            }
        }

        private void Window_Deactivated(object? sender, EventArgs e) =>
            _inputViewModel?.KeyPresses.Clear();

        private bool CanReadInput => IsLoaded && IsVisible && IsEnabled &&
            IsKeyboardFocusWithin && _window?.IsActive == true &&
            ViewModel?.DisplayLocationInfo == false;

        private KeyboardState CaptureKeyboard()
        {
            if (!CanReadInput) return new KeyboardState();
            return new KeyboardState
            {
                UpArrowDown = Keyboard.IsKeyDown(Key.Up),
                DownArrowDown = Keyboard.IsKeyDown(Key.Down),
                LeftArrowDown = Keyboard.IsKeyDown(Key.Left),
                RightArrowDown = Keyboard.IsKeyDown(Key.Right),
                SpaceDown = Keyboard.IsKeyDown(Key.Space)
            };
        }

        private void ExploreAreaView_OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var viewModel = ViewModel;
            if (e.IsRepeat || !CanReadInput || viewModel?.Engine.AnimationRunning != true) return;
            KeyboardKey? key = e.Key switch
            {
                Key.Up => KeyboardKey.UpArrow,
                Key.Down => KeyboardKey.DownArrow,
                Key.Left => KeyboardKey.LeftArrow,
                Key.Right => KeyboardKey.RightArrow,
                Key.Space => KeyboardKey.Space,
                _ => null
            };
            if (key.HasValue) viewModel.KeyPresses.Enqueue(key.Value);
        }
    }
}
