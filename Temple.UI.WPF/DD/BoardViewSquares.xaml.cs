using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Craft.Utils;
using Craft.UI.Utils;
using Temple.ViewModel.DD;

namespace Temple.UI.WPF.DD
{
    /// <summary>
    /// Interaction logic for BoardViewSquares.xaml
    /// </summary>
    public partial class BoardViewSquares : UserControl
    {
        private PointD _mouseDownViewport;
        private PointD _initialScrollOffset;
        private bool _dragging;

        private BoardViewModel ViewModel
        {
            get { return DataContext as BoardViewModel; }
        }

        public BoardViewSquares()
        {
            InitializeComponent();
        }

        private void ScrollViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ViewModel == null) return;

            ViewModel.ScrollableOffset = new PointD(
                ScrollViewer.ScrollableWidth,
                ScrollViewer.ScrollableHeight);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ViewModel == null) return;

            // Is the scroll change caused by a change in the size of the embedded Canvas?
            if (Math.Abs(e.ExtentWidthChange) > 0.0000001 ||
                Math.Abs(e.ExtentHeightChange) > 0.0000001)
            {
                ViewModel.ScrollableOffset = new PointD(
                    ScrollViewer.ScrollableWidth,
                    ScrollViewer.ScrollableHeight);

                ViewModel.ScrollOffset = new PointD(0, 0);

                return;
            }

            // Otherwise we assume it is because the user interacted with a scrollbar 
            ViewModel.ScrollOffset = new PointD(
                e.HorizontalOffset,
                e.VerticalOffset);
        }

        private void Canvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null) return;

            _mouseDownViewport = e.GetPosition(ScrollViewer).AsPointD();
            _initialScrollOffset = ViewModel.ScrollOffset;
            Canvas.CaptureMouse();
            _dragging = true;
        }

        private void Canvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null) return;

            _dragging = false;
            Canvas.ReleaseMouseCapture();

            // The user might have moved a creature, so inform the ViewModel
            ViewModel.PlayerClickedOnBoard();
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (ViewModel == null) return;

            if (_dragging)
            {
                var scrollViewerPositionForMouseMove = e.GetPosition(ScrollViewer).AsPointD();
                var offset = scrollViewerPositionForMouseMove - _mouseDownViewport;

                ViewModel.ScrollOffset = new PointD(
                    Math.Min(Math.Max(0, _initialScrollOffset.X - offset.X), ViewModel.ScrollableOffset.X),
                    Math.Min(Math.Max(0, _initialScrollOffset.Y - offset.Y), ViewModel.ScrollableOffset.Y));
            }
            else
            {
                var canvasPositionForMouseMove = e.GetPosition(Canvas).AsPointD();
                ViewModel.MousePositionWorld.Object = canvasPositionForMouseMove / ViewModel.Magnification;
            }
        }

        private void MoveCreatureStoryboard_Completed(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            CurrentCreatureGrid.RenderTransform = new TranslateTransform(0, 0);

            ViewModel.CompleteMoveCreatureAnimation();
        }

        private void AttackStoryboard_Completed(object sender, EventArgs e)
        {
            if (ViewModel == null) return;

            WeaponGrid.RenderTransform = new TranslateTransform(0, 0);

            ViewModel.CompleteAttackAnimation();
        }
    }
}
