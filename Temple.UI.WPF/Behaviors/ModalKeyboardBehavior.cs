using System.Windows;
using System.Windows.Input;

// (Made by Codex)
namespace Temple.UI.WPF.Behaviors
{
    /// <summary>
    /// Blocks keyboard input to a view while a modal overlay is active,
    /// leaving Enter available to WPF's default button handling.
    /// </summary>
    public static class ModalKeyboardBehavior
    {
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached(
                "IsActive",
                typeof(bool),
                typeof(ModalKeyboardBehavior),
                new PropertyMetadata(false, OnIsActiveChanged));

        public static bool GetIsActive(DependencyObject element) =>
            (bool)element.GetValue(IsActiveProperty);

        public static void SetIsActive(DependencyObject element, bool value) =>
            element.SetValue(IsActiveProperty, value);

        private static void OnIsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not UIElement element)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                element.PreviewKeyDown += OnPreviewKeyDown;
                element.PreviewKeyUp += OnPreviewKeyUp;
            }
            else
            {
                element.PreviewKeyDown -= OnPreviewKeyDown;
                element.PreviewKeyUp -= OnPreviewKeyUp;
            }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Suppress gameplay and navigation before the view's bubbling key handlers.
            if (e.Key != Key.Enter || e.IsRepeat)
            {
                e.Handled = true;
            }
        }

        private static void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
