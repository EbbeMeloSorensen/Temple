using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Temple.UI.WPF.Behaviors
{
    public static class FocusOnLoadBehavior
    {
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached(
                "Enabled",
                typeof(bool),
                typeof(FocusOnLoadBehavior),
                new PropertyMetadata(false, OnEnabledChanged));

        public static void SetEnabled(DependencyObject obj, bool value) =>
            obj.SetValue(EnabledProperty, value);

        public static bool GetEnabled(DependencyObject obj) =>
            (bool)obj.GetValue(EnabledProperty);

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContentControl cc)
            {
                var descriptor = DependencyPropertyDescriptor.FromProperty(
                    ContentControl.ContentProperty, typeof(ContentControl));

                if (descriptor == null)
                    return;

                if ((bool)e.NewValue)
                    descriptor.AddValueChanged(cc, OnContentChanged);
                else
                    descriptor.RemoveValueChanged(cc, OnContentChanged);

                // Also handle initial load
                cc.Loaded -= Cc_Loaded;
                cc.Loaded += Cc_Loaded;
            }
        }

        private static void Cc_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ContentControl cc)
                TryFocusContent(cc);
        }

        private static void OnContentChanged(object sender, EventArgs e)
        {
            if (sender is ContentControl cc)
                TryFocusContent(cc);
        }

        private static void TryFocusContent(ContentControl cc)
        {
            if (cc == null)
                return;

            // Defer so the template is applied and visuals are built
            cc.Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    var vm = cc.Content;
                    if (vm == null) return;

                    // Find the view that represents this VM (not the ContentPresenter)
                    var view = FindViewForViewModel(cc, vm);
                    if (view == null) return;

                    // Make sure it can receive focus
                    view.Focusable = true;
                    if (view is Control ctrl && ctrl.Background == null)
                        ctrl.Background = System.Windows.Media.Brushes.Transparent;

                    // Give keyboard focus (defer slightly to ensure focusable state)
                    view.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        view.Focus();
                        Keyboard.Focus(view);
                    }), DispatcherPriority.Input);
                }
                catch
                {
                    // swallow exceptions - don't crash UI
                }
            }), DispatcherPriority.Loaded);
        }

        private static FrameworkElement FindViewForViewModel(DependencyObject parent, object viewModel)
        {
            // Recursively search the visual tree for a FrameworkElement whose
            // DataContext is the viewModel and which is NOT a ContentPresenter.
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement fe)
                {
                    // If this child's DataContext is the view model and is not a ContentPresenter,
                    // assume this is the real view we want.
                    if (ReferenceEquals(fe.DataContext, viewModel) && !(fe is ContentPresenter))
                        return fe;

                    // If this child is a ContentPresenter and it hosts the content, we should
                    // descend into it (the template root is usually its visual child).
                    if (fe is ContentPresenter)
                    {
                        // descend into presenter children to find the template root
                        var fromPresenter = FindViewForViewModel(fe, viewModel);
                        if (fromPresenter != null)
                            return fromPresenter;
                    }

                    // Otherwise recurse normally
                    var deeper = FindViewForViewModel(child, viewModel);
                    if (deeper != null)
                        return deeper;
                }
                else
                {
                    // Non-FrameworkElement child - still recurse
                    var deeper = FindViewForViewModel(child, viewModel);
                    if (deeper != null)
                        return deeper;
                }
            }

            return null;
        }
    }
}
