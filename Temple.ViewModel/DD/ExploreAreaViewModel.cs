using GalaSoft.MvvmLight.Command;
using Craft.Utils;
using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
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

            var scene = GenerateScene();

            GeometryEditorViewModel.InitializeWorldWindow(
                scene.InitialWorldWindowFocus(),
                scene.InitialWorldWindowSize(),
                false);

            _sceneViewController.ActiveScene = scene;
        }

        public void StartAnimation()
        {
            Engine.StartOrResumeAnimation();
        }

        private Scene GenerateScene()
        {
            var ballRadius = 0.16;
            var initialBallPosition = new Vector2D(0, 0);

            var initialState = new State();
            initialState.AddBodyState(
                new BodyStateClassic(new CircularBody(1, ballRadius, 1, false), initialBallPosition)
                {
                    Orientation = Math.PI
                });

            var name = "Exploration";
            var standardGravity = 0.0;
            var initialWorldWindowUpperLeft = new Point2D(-1.4, -1.3);
            var initialWorldWindowLowerRight = new Point2D(5, 3);
            var gravitationalConstant = 0.0;
            var coefficientOfFriction = 0.0;
            var timeFactor = 1.0;
            var handleBodyCollisions = false;
            var deltaT = 0.001;
            var viewMode = SceneViewMode.FocusOnFirstBody;

            var scene = new Scene(
                name,
                initialWorldWindowUpperLeft,
                initialWorldWindowLowerRight,
                initialState,
                standardGravity,
                gravitationalConstant,
                coefficientOfFriction,
                timeFactor,
                handleBodyCollisions,
                deltaT,
                viewMode);

            scene.CollisionBetweenBodyAndBoundaryOccuredCallBack =
                body => OutcomeOfCollisionBetweenBodyAndBoundary.Block;

            scene.InteractionCallBack = (keyboardState, keyboardEvents, mouseClickPosition, collisions, currentState) =>
            {
                var currentStateOfMainBody = currentState.BodyStates.First() as BodyStateClassic;
                var currentRotationalSpeed = currentStateOfMainBody.RotationalSpeed;
                var currentArtificialSpeed = currentStateOfMainBody.ArtificialVelocity.Length;

                var newRotationalSpeed = 0.0;

                if (keyboardState.LeftArrowDown)
                {
                    newRotationalSpeed += Math.PI;
                }

                if (keyboardState.RightArrowDown)
                {
                    newRotationalSpeed -= Math.PI;
                }

                var newArtificialSpeed = 0.0;

                if (keyboardState.UpArrowDown)
                {
                    newArtificialSpeed += 1.5;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 1.5;
                }

                currentStateOfMainBody.RotationalSpeed = newRotationalSpeed;
                currentStateOfMainBody.ArtificialVelocity = new Vector2D(newArtificialSpeed, 0);

                if (Math.Abs(newRotationalSpeed - currentRotationalSpeed) < 0.01 &&
                    Math.Abs(newArtificialSpeed - currentArtificialSpeed) < 0.01)
                {
                    return false;
                }

                return true;
            };

            // Liniestykker defineres i et normalt xy koordinatsystem
            var lineSegments = new List<LineSegment2D>
            {
                new(new Point2D(-2, 2), new Point2D(-2, 0)),
                new(new Point2D(-3, 0), new Point2D(-3, -4)),
                new(new Point2D(-2, 0), new Point2D(-3, 0)),
                new(new Point2D(2, 2), new Point2D(-2, 2)),
                new(new Point2D(2, -2), new Point2D(2, 2)),
                new(new Point2D(-2, -2), new Point2D(2, -2)),
            };

            //var group = new Model3DGroup();
            //var material = new DiffuseMaterial(new SolidColorBrush(Colors.Orange));

            foreach (var lineSegment in lineSegments)
            {
                scene.AddBoundary(new LineSegment(
                    new Vector2D(lineSegment.Point1.X, -lineSegment.Point1.Y),
                    new Vector2D(lineSegment.Point2.X, -lineSegment.Point2.Y)));

                //var rectangleMesh = CreateWall(
                //    new Point2D(lineSegment.Point1.Y, lineSegment.Point1.X),
                //    new Point2D(lineSegment.Point2.Y, lineSegment.Point2.X));

                //var rectangleModel = new GeometryModel3D(rectangleMesh, material);
                //group.Children.Add(rectangleModel);
            }

            //Scene3D = group;

            return scene;
        }
    }
}
