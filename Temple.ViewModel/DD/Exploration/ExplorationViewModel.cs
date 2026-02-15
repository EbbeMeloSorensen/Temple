using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using GalaSoft.MvvmLight.Command;
using System.Windows.Media.Media3D;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Interfaces.Readers;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Common;
using Temple.Infrastructure.GameConditions;
using Temple.Infrastructure.Presentation;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Scene = Craft.Simulation.Scene;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Temple.ViewModel.DD.Exploration
{
    public class ExplorationViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;
        private SceneViewController _sceneViewController;
        private readonly ISiteRenderer _siteRenderer;
        private readonly IQuestStatusReader _questStatusReadModel;
        private readonly IGameQueryService _gameQueryService;

        private Model3D _scene3D;
        private Point3D _cameraPosition;
        private Vector3D _lookDirection;
        private Point3D _playerLightPosition;
        private Vector3D _directionalLight;

        public Engine Engine { get; }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }

        public Model3D Scene3D
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

        public Vector3D LookDirection
        {
            get => _lookDirection;
            set
            {
                _lookDirection = value;
                RaisePropertyChanged();
            }
        }

        public Point3D PlayerLightPosition
        {
            get => _playerLightPosition;
            set
            {
                _playerLightPosition = value;
                RaisePropertyChanged();
            }
        }

        public Vector3D DirectionalLight
        {
            get => _directionalLight;
            set
            {
                _directionalLight = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoToInGameMenu_Command { get; }

        public ExplorationViewModel(
            ApplicationController controller,
            IQuestStatusReader questStatusReadModel,
            ISiteRenderer siteRenderer,
            IGameQueryService gameQueryService)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _questStatusReadModel = questStatusReadModel ?? throw new ArgumentNullException(nameof(questStatusReadModel));
            _siteRenderer = siteRenderer ?? throw new ArgumentNullException(nameof(siteRenderer));
            _gameQueryService = gameQueryService;

            Engine = new Engine(null);

            GoToInGameMenu_Command = new RelayCommand(() =>
            {
                Engine.HandleClosing();

                var payload = new InGameMenuPayload
                {
                    PayloadForNextState = new ExplorationPayload
                    {
                        SiteId = _controller.ApplicationData.CurrentSiteId
                    }
                };

                _controller.GoToNextApplicationState(payload);
            });

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

                switch (bs)
                {
                    case BodyStateClassic bsc:
                    {
                        var orientation = bsc.Orientation;

                        return new RotatableEllipseViewModel
                        {
                            Width = 2 * circularBody.Radius,
                            Height = 2 * circularBody.Radius,
                            Orientation = orientation
                        };
                    }
                    case BodyState:
                    {
                        return new EllipseViewModel
                        {
                            Width = 2 * circularBody.Radius,
                            Height = 2 * circularBody.Radius,
                        };
                    }
                    default:
                    {
                        throw new NotSupportedException();
                    }
                }
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

                if (outcome.Length >= 3 && outcome.Substring(0, 3) == "NPC")
                {
                    var payload = new DialoguePayload
                    {
                        NPCId = outcome.Substring(4)
                    };

                    _controller.GoToNextApplicationState(payload);
                }
                else if (outcome == "Exit_Wilderness")
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
                        PayloadForNextStateInCasePartyWins = new ExplorationPayload { SiteId = _controller.ApplicationData.CurrentSiteId }
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

            _controller.ApplicationData.CurrentSiteId = explorationPayload.SiteId;

            if (_controller.ApplicationData.ExplorationPosition == null ||
                _controller.ApplicationData.ExplorationOrientation == null)
            {
                throw new InvalidOperationException("Position and orientation needed here");
            }

            var siteData = SiteDataFactory.GenerateSiteData(
                explorationPayload.SiteId,
                _questStatusReadModel);

            Scene3D = ((WpfSiteModel)_siteRenderer.Build(siteData)).Model3D;

            var scene = ExplorationSceneFactory.GenerateScene(
                siteData,
                _controller.ApplicationData.ExplorationPosition,
                _controller.ApplicationData.ExplorationOrientation.Value,
                _controller.ApplicationData.BattlesWon,
                _gameQueryService);

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

                _controller.ApplicationData.ExplorationPosition = position;
                _controller.ApplicationData.ExplorationOrientation = orientation;

                CameraPosition = new Point3D(
                    -position.Y,
                    0.5,
                    position.X);

                LookDirection = new Vector3D(Math.Sin(orientation), 0, Math.Cos(orientation));
                DirectionalLight = LookDirection + new Vector3D(0, -0.5, 0);
                PlayerLightPosition = CameraPosition + LookDirection * 3 + new Vector3D(0, -1, 0);
            };

            Engine.StartOrResumeAnimation();
        }
    }
}
