using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using System.Windows.Media.Media3D;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Scene = Craft.Simulation.Scene;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Temple.ViewModel.DD.Exploration
{
    public class ExplorationViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;
        private SceneViewController _sceneViewController;
        private readonly ISceneRenderer _sceneRenderer;

        private Model3DGroup _scene3D;
        private Point3D _cameraPosition;
        private Point3D _lightPosition;
        private Vector3D _lookDirection;

        public Engine Engine { get; }
        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public Model3DGroup Scene3D
        {
            get => _scene3D;
            private set
            {
                _scene3D = value;
                RaisePropertyChanged();
            }
        }

        public Point3D CameraPosition
        {
            get => _cameraPosition;
            set
            {
                _cameraPosition = value;
                RaisePropertyChanged();
            }
        }

        public Point3D LightPosition
        {
            get => _lightPosition;
            set
            {
                _lightPosition = value;
                RaisePropertyChanged();
            }
        }

        public Vector3D LookDirection
        {
            get => _lookDirection;
            set
            {
                _lookDirection = value;
                RaisePropertyChanged();
            }
        }

        public ExplorationViewModel(
            ApplicationController controller,
            ISceneRenderer sceneRenderer)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _sceneRenderer = sceneRenderer ?? throw new ArgumentNullException(nameof(sceneRenderer));

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
                var outcome = Engine.EngineCore.Outcome as string;

                if (outcome == "Exit_To_Wilderness")
                {
                    _controller.GoToWilderness();
                }
                else
                {
                    string battleId;
                    string? entranceId;

                    if (outcome.Contains(';'))
                    {
                        var separatorIndex = outcome.IndexOf(';');
                        battleId = outcome.Substring(0, separatorIndex);
                        entranceId = outcome.Substring(separatorIndex + 1);
                    }
                    else
                    {
                        battleId = outcome;
                        entranceId = null;
                    }

                    var payload = new BattlePayload
                    {
                        BattleId = battleId,
                        EntranceId = entranceId,
                        PayloadForNextStateInCasePartyWins = new ExplorationPayload { Site = _controller.Data.CurrentSite }
                    };

                    _controller.GoToNextApplicationState(payload);
                }
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

            var siteSpecs = ExplorationSceneFactory.GetSiteSpecs(explorationPayload.Site);

            Scene3D = ExplorationSceneFactory.Generate3DScene(
                siteSpecs);

            var scene = ExplorationSceneFactory.GenerateScene(
                siteSpecs,
                _controller.Data.ExplorationPosition,
                _controller.Data.ExplorationOrientation.Value,
                _controller.Data.BattlesWon);

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

            Engine.CurrentStateChanged += (s, e) =>
            {
                var bodyStateOfProtagonist = e.State.BodyStates.First() as BodyStateClassic;
                var position = bodyStateOfProtagonist.Position;
                var orientation = bodyStateOfProtagonist.Orientation;

                _controller.Data.ExplorationPosition = position;
                _controller.Data.ExplorationOrientation = orientation;

                CameraPosition = new Point3D(
                    -position.Y,
                    0.5,
                    position.X);

                LightPosition = new Point3D(
                    -position.Y,
                    0.5,
                    position.X);

                LookDirection = new Vector3D(Math.Sin(orientation), 0, Math.Cos(orientation));
            };

            Engine.StartOrResumeAnimation();
        }
    }
}
