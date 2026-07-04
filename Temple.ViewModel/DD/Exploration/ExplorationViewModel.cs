using Craft.Logging;
using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Engine;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.Reborn.GeometricModels;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Media.Media3D;
using Craft.Simulation.Boundaries;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Battle;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.Presentation;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Scene = Craft.Simulation.Scene;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Temple.ViewModel.DD.Exploration
{
    public class ExplorationViewModel : TempleViewModel, IFrameAware
    {
        private readonly ApplicationController _controller;
        private SceneViewController _sceneViewController;
        private GeometryDataStore _geometryDataStore;
        private readonly ISiteDataFactory _siteDataFactory;
        private readonly ISiteRenderer _siteRenderer;
        private readonly IGameQueryService _gameQueryService;

        private Scene _scene2D;
        private Model3D _scene3D;
        private Point3D _cameraPosition;
        private Vector3D _lookDirection;
        private Point3D _playerLightPosition;
        private Vector3D _directionalLight;

        public Engine Engine { get; }

        public GeometryEditorViewModel GeometryEditorViewModel { get; }
        public GeometryViewModel GeometryViewModel { get; }

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
            ISiteDataFactory siteDataFactory,
            ISiteRenderer siteRenderer,
            IGameQueryService gameQueryService)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _siteDataFactory = siteDataFactory ?? throw new ArgumentNullException(nameof(siteDataFactory));
            _siteRenderer = siteRenderer ?? throw new ArgumentNullException(nameof(siteRenderer));
            _gameQueryService = gameQueryService ?? throw new ArgumentNullException(nameof(gameQueryService)); ;

            Engine = new Engine(new DummyLogger());

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
                //UpdateModelCallBack = Engine.UpdateModel
            };

            GeometryViewModel = new GeometryViewModel()
            {
                ShowCoordinateSystem = true,
                LockAspectRatio = true,
                DampFocusShifts = false
            };

            GeometryViewModel.PropertyChanged += GeometryViewModel_PropertyChanged;
            Engine.CurrentStateChanged += Engine_CurrentStateChanged;

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

            var siteData = _siteDataFactory.GenerateSiteData(
                explorationPayload.SiteId);

            var temp = siteData.SiteComponents
                .Where(_ => _.Condition == null ||
                            _.Condition.Evaluate(_gameQueryService));

            siteData = new SiteData
            {
                SiteComponents = siteData.SiteComponents
                    .Where(_ => _.Condition == null ||
                                _.Condition.Evaluate(_gameQueryService))
                    .ToList(),
            };

            Scene3D = ((WpfSiteModel)_siteRenderer.Build(siteData)).Model3D;

            _scene2D = ExplorationSceneFactory.GenerateScene(
                siteData,
                _controller.ApplicationData.ExplorationPosition,
                _controller.ApplicationData.ExplorationOrientation.Value,
                _controller.ApplicationData.BattlesWon,
                _gameQueryService);

            InitializeGeometryDataStore(_scene2D);

            StartAnimation(_scene2D);

            return this;
        }

        public void OnFrame(
            TimeSpan time,
            double dt)
        {
            // Bemærk, at man ikke bruger parametrene her
            Engine.UpdateModel();
        }

        private void InitializeGeometryDataStore(
            Scene scene)
        {
            var staticGeometryObjects = new List<object>();

            scene.Boundaries.ForEach(boundary =>
            {
                if (!boundary.Visible) return;

                switch (boundary)
                {
                    case HorizontalLineSegment horizontalLineSegment:
                        staticGeometryObjects.Add(new LineModel
                        {
                            P1 = new Point(horizontalLineSegment.X0, horizontalLineSegment.Y),
                            P2 = new Point(horizontalLineSegment.X1, horizontalLineSegment.Y)
                        });
                        break;
                    case VerticalLineSegment verticalLineSegment:
                        staticGeometryObjects.Add(new LineModel
                        {
                            P1 = new Point(verticalLineSegment.X, verticalLineSegment.Y0),
                            P2 = new Point(verticalLineSegment.X, verticalLineSegment.Y1)
                        });
                        break;
                    case LineSegment lineSegment:
                        staticGeometryObjects.Add(new LineModel
                        {
                            P1 = new Point(lineSegment.Point1.X, lineSegment.Point1.Y),
                            P2 = new Point(lineSegment.Point2.X, lineSegment.Point2.Y)
                        });
                        break;
                    case BoundaryPoint boundaryPoint:
                        staticGeometryObjects.Add(new PointModel()
                        {
                            P = new Point(boundaryPoint.Point.X, boundaryPoint.Point.Y)
                        });
                        break;
                    case CircularBoundary circularBoundary:
                        staticGeometryObjects.Add(new CircleModel()
                        {
                            Center = new Point(circularBoundary.Center.X, circularBoundary.Center.Y),
                            Radius = circularBoundary.Radius
                        });
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

            var boundingBoxes = staticGeometryObjects.Select(geometryObject =>
            {
                return geometryObject switch
                {
                    LineModel line => line.ComputeBoundingBox(),
                    PointModel point => point.ComputeBoundingBox(),
                    CircleModel circle => circle.ComputeBoundingBox(),
                    _ => throw new InvalidOperationException(),
                };
            });

            _geometryDataStore = new GeometryDataStore(
                new Craft.DataStructures.Geometry.BoundingBox(
                    boundingBoxes.Min(b => b.MinX),
                    boundingBoxes.Max(b => b.MaxX),
                    boundingBoxes.Min(b => b.MinY),
                    boundingBoxes.Max(b => b.MaxY)));

            staticGeometryObjects.ForEach(_geometryDataStore.AddStaticGeometryObject);
        }

        private void StartAnimation(
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

        private void GeometryViewModel_PropertyChanged(
            object? sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GeometryViewModel.WorldWindowExpanded))
            {
                UpdateStaticGeometricObjects();
            }
        }

        private void Engine_CurrentStateChanged(
            object? sender,
            CurrentStateChangedEventArgs e)
        {
            UpdateGeometricObjects(e.State);

            if (_scene2D.ViewMode == SceneViewMode.FocusOnFirstBody)
            {
                UpdateFocus(e.State.BodyStates.First().Position);
            }
        }

        private void UpdateStaticGeometricObjects()
        {
            GeometryViewModel.ClearLayer(false);

            if (_geometryDataStore != null)
            {
                GeometryViewModel.AddStaticGeometryLayer(
                    _geometryDataStore.Query(GeometryViewModel.WorldWindowExpanded));
            }
        }

        private void UpdateGeometricObjects(
            State state)
        {
            var geometricObjects = state.BodyStates.Select(bs => new CircleModel
            {
                Center = new Point(bs.Position.X, bs.Position.Y),
                Radius = (bs.Body as CircularBody)!.Radius
            });

            GeometryViewModel.ReplaceDynamicGeometryLayer(geometricObjects);
        }

        private void UpdateFocus(
            Vector2D focus)
        {
            // Dette trigger, at den crasher, når man skifter til game menu - du skal måske have en guard mere i Craft
            //GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
            //{
            //    WorldPoint = new Point(focus.X, focus.Y),
            //    ViewportRatio = new Size(0.5, 0.5)
            //};
        }
    }
}
