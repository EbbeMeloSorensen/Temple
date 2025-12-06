using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using Temple.Application.Core;
using Temple.Application.State;

namespace Temple.ViewModel.DD
{
    public class ExploreAreaViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;
        private SceneViewController _sceneViewController;
        private ObservableObject<string> _next;

        public Engine Engine { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public RelayCommand ContinueCommand { get; }

        public ExploreAreaViewModel(
            ApplicationController controller,
            ObservableObject<string> next)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _next = next ?? throw new ArgumentNullException(nameof(next));

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
                // Todo: Ikke antag, at det er en string
                _next.Object = Engine.EngineCore.Outcome as string;
                _controller.ExitState();
            };
        }

        public override TempleViewModel Init(
            ApplicationStatePayload payload)
        {
            var interludePayload = payload as ExplorationPayload
                                   ?? throw new ArgumentException("Payload is not of type ExplorationPayload", nameof(payload));

            //Text = interludePayload.Text;

            //_payloadForNextState = interludePayload.PayloadForNextState;

            return this;
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
