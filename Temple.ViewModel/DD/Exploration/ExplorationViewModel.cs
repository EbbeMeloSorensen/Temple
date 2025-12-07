using Craft.Math;
using Craft.Utils;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.Simulation.Engine;
using Craft.Utils.Linq;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.Exploration
{
    public class ExplorationViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;
        private SceneViewController _sceneViewController;

        public Engine Engine { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public ExplorationViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

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
                var payload = new BattlePayload
                {
                    BattleId = Engine.EngineCore.Outcome as string,
                    PayloadForNextStateInCasePartyWins = new ExplorationPayload{Area = "Dungeon1"}
                };

                _controller.GoToNextApplicationState(payload);
            };
        }

        public override TempleViewModel Init(
            ApplicationStatePayload payload)
        {
            var explorationPayload = payload as ExplorationPayload
                                     ?? throw new ArgumentException("Payload is not of type ExplorationPayload", nameof(payload));

            if (_controller.Data.ExplorationPosition == null ||
                _controller.Data.ExplorationOrientation == null)
            {
                throw new InvalidOperationException("Position and orientation needed here");
            }

            var scene = GenerateScene(
                _controller.Data.ExplorationPosition,
                _controller.Data.ExplorationOrientation.Value);

            StartAnimation(scene);

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

        private Scene GenerateScene(
            Vector2D initialPositionOfParty,
            double initialOrientationOfParty)
        {
            var ballRadius = 0.16;

            var initialState = new State();

            initialState.AddBodyState(
                new BodyStateClassic(new CircularBody(1, ballRadius, 1, false), position: initialPositionOfParty)
                {
                    Orientation = initialOrientationOfParty
                });

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
                null,
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
                    newArtificialSpeed += 3.0;
                }

                if (keyboardState.DownArrowDown)
                {
                    newArtificialSpeed -= 3.0;
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

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var currentStateOfMainBody = propagatedState.TryGetBodyState(1) as BodyStateClassic;

                if (currentStateOfMainBody == null)
                {
                    throw new InvalidOperationException("Expected a bodystate here");
                }

                _controller.Data.ExplorationPosition = currentStateOfMainBody.Position;
                _controller.Data.ExplorationOrientation = currentStateOfMainBody.Orientation;

                var response = new PostPropagationResponse();

                if (!boundaryCollisionReports.Any()) return response;

                var boundary = boundaryCollisionReports.First().Boundary;

                if (string.IsNullOrEmpty(boundary.Tag)) return response;

                response.Outcome = boundary.Tag;
                response.IndexOfLastState = propagatedState.Index;

                return response;
            };

            // Liniestykker defineres i et normalt xy koordinatsystem
            var lineSegments = new List<LineSegment2D>
            {
                new(new Point2D(0, 2), new Point2D(0, 0)),
                new(new Point2D(0, 0), new Point2D(1, 0)),
                new(new Point2D(1, 0), new Point2D(1, 5)),
                new(new Point2D(1, 5), new Point2D(3, 5)),
                new(new Point2D(3, 5), new Point2D(3, 9)),
                new(new Point2D(3, 9), new Point2D(-2, 9)),
                new(new Point2D(-2, 9), new Point2D(-2, 5)),
                new(new Point2D(-2, 5), new Point2D(0, 5)),
                new(new Point2D(0, 5), new Point2D(0, 3)),
                new(new Point2D(0, 3), new Point2D(-1, 3)),
                new(new Point2D(-1, 3), new Point2D(-1, 4)),
                new(new Point2D(-1, 4), new Point2D(-3, 4)),
                new(new Point2D(-3, 4), new Point2D(-3, 1)),
                new(new Point2D(-3, 1), new Point2D(-1, 1)),
                new(new Point2D(-1, 1), new Point2D(-1, 2)),
                new(new Point2D(-1, 2), new Point2D(0, 2)),
            };

            var wallPolyLines = new List<List<Point2D>>
            {
                new()
                {
                    new (0, 0),
                    new (1, 0),
                    new (1, 2),
                    new (2, 2),
                    new (2, 1),
                    new (4, 1),
                    new (4, 2),
                    new (5, 2),
                    new (5, 1),
                    new (7, 1),
                    new (7, 6),
                    new (8, 6),
                    new (8, 9),
                    new (5, 9),
                    new (5, 7),
                    new (3, 7),
                    new (3, 9),
                    new (-2, 9),
                    new (-2, 5),
                    new (0, 5),
                    new (0, 3),
                    new (-1, 3),
                    new (-1, 4),
                    new (-3, 4),
                    new (-3, 1),
                    new (-1, 1),
                    new (-1, 2),
                    new (0, 2),
                    new (0, 0)
                },
                new ()
                {
                    new (1, 3),
                    new (1, 5),
                    new (3, 5),
                    new (3, 6),
                    new (6, 6),
                    new (6, 5),
                    new (5, 5),
                    new (5, 3),
                    new (4, 3),
                    new (4, 4),
                    new (2, 4),
                    new (2, 3),
                    new (1, 3),
                }
            };

            //var group = new Model3DGroup();
            //var material = new DiffuseMaterial(new SolidColorBrush(Colors.Orange));

            wallPolyLines.ForEach(wallPolyLine =>
            {
                wallPolyLine.AdjacentPairs().ToList().ForEach(_ =>
                {
                    scene.AddBoundary(new LineSegment(
                        new Vector2D(_.Item1.X, -_.Item1.Y),
                        new Vector2D(_.Item2.X, -_.Item2.Y)));

                    //var rectangleMesh = CreateWall(
                    //    new Point2D(lineSegment.Point1.Y, lineSegment.Point1.X),
                    //    new Point2D(lineSegment.Point2.Y, lineSegment.Point2.X));

                    //var rectangleModel = new GeometryModel3D(rectangleMesh, material);
                    //group.Children.Add(rectangleModel);
                });
            });


            //Scene3D = group;

            AddBattleUnlessWon("Dungeon 1, Room 1, Goblin", scene, new Vector2D(-1, -3), new Vector2D(-1, -2));
            AddBattleUnlessWon("Final Battle", scene, new Vector2D(1, -5), new Vector2D(0, -5));

            return scene;
        }

        private void AddBattleUnlessWon(
            string battleId,
            Scene scene,
            Vector2D point1,
            Vector2D point2)
        {
            if (!_controller.Data.BattlesWon.Contains(battleId))
            {
                scene.AddBoundary(new LineSegment(point1, point2, battleId));
            }
        }
    }
}
