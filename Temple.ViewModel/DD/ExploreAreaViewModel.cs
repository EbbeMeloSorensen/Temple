using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using Temple.Application.Core;

namespace Temple.ViewModel.DD
{
    public class ExploreAreaViewModel
    {
        private readonly ApplicationController _controller;
        private SceneViewController _sceneViewController;

        public Engine Engine { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public RelayCommand ContinueCommand { get; }

        public ExploreAreaViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            ContinueCommand = new RelayCommand(() =>
            {
                Engine.HandleClosing(); // Find lige et bedre navn, hva..

                _controller.ExitState();
            });

            Engine = new Engine(null);

            GeometryEditorViewModel = new GeometryEditorViewModel(1)
            {
                UpdateModelCallBack = Engine.UpdateModel
            };

            ShapeSelectorCallback shapeSelectorCallback = (bs) =>
            {
                if (!(bs.Body is CircularBody))
                {
                    throw new InvalidOperationException();
                }

                var circularBody = bs.Body as CircularBody;

                var bsc = bs as BodyStateClassic;
                var orientation = bsc == null ? 0 : bsc.Orientation;

                return new RotatableEllipseViewModel
                {
                    Width = 2 * circularBody.Radius,
                    Height = 2 * circularBody.Radius,
                    Orientation = orientation
                };
            };

            ShapeUpdateCallback shapeUpdateCallback = (shapeViewModel, bs) =>
            {
                // Her opdaterer vi POSITIONEN af shapeviewmodellen
                shapeViewModel.Point = new PointD(bs.Position.X, bs.Position.Y);

                // Her opdaterer vi ORIENTERINGEN af shapeviewmodellen
                if (shapeViewModel is RotatableEllipseViewModel)
                {
                    var bsc = bs as BodyStateClassic;
                    var orientation = bsc == null ? 0 : bsc.Orientation;

                    var rotatableEllipseViewModel = shapeViewModel as RotatableEllipseViewModel;
                    rotatableEllipseViewModel.Orientation = orientation;
                }
            };

            _sceneViewController = new SceneViewController(
                Engine,
                GeometryEditorViewModel,
                shapeSelectorCallback,
                shapeUpdateCallback);

            Engine.AnimationCompleted += (s, e) =>
            {
                var outcome = Engine.EngineCore.Outcome;
                var a = 0;
                ContinueCommand.Execute(null);
                //_controller.ExitState();
            };
        }

        public void StartAnimation(
            Scene scene)
        {
            GeometryEditorViewModel.InitializeWorldWindow(
                scene.InitialWorldWindowFocus(),
                scene.InitialWorldWindowSize(),
                false);

            _sceneViewController.ActiveScene = scene;

            Engine.StartOrResumeAnimation();
        }
    }
}
