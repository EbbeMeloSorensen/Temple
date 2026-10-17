using System.Collections;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using GalaSoft.MvvmLight.Command;
using Craft.Logging;
using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.Simulation.Engine;
using Craft.Utils;
using Craft.ViewModels.Geometry2D.Reborn;
using Craft.ViewModels.Geometry2D.ScrollFree;
using Craft.ViewModels.Simulation;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Geometry;
using Temple.Infrastructure.Presentation;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Scene = Craft.Simulation.Scene;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace Temple.ViewModel.DD.Exploration
{
    public class ExplorationViewModel : TempleViewModel, IFrameAware
    {
        private Material _materialDoor;

        private readonly ApplicationController _controller;
        private GeometryDataStore _geometryDataStore;
        private readonly ISiteDataFactory _siteDataFactory;
        private readonly ISiteRenderer _siteRenderer;
        private readonly IGameQueryService _gameQueryService;

        private State _currentState;
        private Scene _scene2D;
        private Model3D _scene3DStatic;
        private Model3D _scene3DDynamic;
        private Point3D _cameraPosition;
        private Vector3D _lookDirection;
        private Point3D _playerLightPosition;
        private Vector3D _directionalLight;

        private Dictionary<string, string>? _locationInfoDictionary;
        private string _locationInfo;
        private bool _displayLocationIfo;

        private Dictionary<int, DoorRotationViewModel> DoorRotationViewModelDictionary { get; }
        private readonly DoorGeometryCache _doorGeometryCache = new();
        private readonly ArrayList _dynamicGeometricObjects = new();
        private readonly GeometryLayer _dynamicGeometryLayer;

        public Engine Engine { get; }

        public GeometryViewModel GeometryViewModel { get; }

        public Model3D Scene3DStatic
        {
            get => _scene3DStatic;
            private set
            {
                _scene3DStatic = value;
                RaisePropertyChanged();
            }
        }

        public Model3D Scene3DDynamic
        {
            get => _scene3DDynamic;
            private set
            {
                _scene3DDynamic = value;
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

        public string LocationInfo
        {
            get => _locationInfo;
            set
            {
                _locationInfo = value;
                RaisePropertyChanged();
            }
        }

        public bool DisplayLocationInfo
        {
            get => _displayLocationIfo;
            set
            {
                _displayLocationIfo = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand GoToInGameMenu_Command { get; }

        public RelayCommand CloseLocationInfo_Command { get; }

        public ExplorationViewModel(
            ApplicationController controller,
            ISiteDataFactory siteDataFactory,
            ISiteRenderer siteRenderer,
            IGameQueryService gameQueryService)
        {
            _materialDoor = new DiffuseMaterial(new SolidColorBrush(Colors.SandyBrown));

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

            CloseLocationInfo_Command = new RelayCommand(() =>
            {
                var payload = new ExplorationPayload
                {
                    SiteId = _controller.ApplicationData.CurrentSiteId
                };

                _controller.GoToNextApplicationState(payload);
            });

            GeometryViewModel = new GeometryViewModel()
            {
                ShowCoordinateSystem = false,
                LockAspectRatio = true,
                DampFocusShifts = false
            };

            DoorRotationViewModelDictionary = new Dictionary<int, DoorRotationViewModel>();
            _dynamicGeometryLayer = new GeometryLayer(_dynamicGeometricObjects, true);
            GeometryViewModel.GeometryLayers.Add(_dynamicGeometryLayer);

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

            Engine.AnimationCompleted += (s, e) =>
            {
                var outcome = Engine.EngineCore.Outcome as string;

                if (outcome.Length >= 4 && outcome.Substring(0, 4) == "info")
                {
                    // Show Info (don´t switch to another state)

                    var locationInfoId = outcome.Substring(6);

                    controller.EventBus.Publish(new KnowledgeGainedEvent(locationInfoId));

                    LocationInfo = _locationInfoDictionary.ContainsKey(locationInfoId)
                        ? _locationInfoDictionary[locationInfoId]
                        : "location info id not found in site data";

                    DisplayLocationInfo = true;
                }
                else if (outcome.Length >= 3 && outcome.Substring(0, 3) == "npc")
                {
                    var payload = new DialoguePayload
                    {
                        NPCId = outcome.Substring(4)
                    };

                    _controller.GoToNextApplicationState(payload);
                }
                else if (outcome.Length >= 4 && outcome.Substring(0, 4) == "exit")
                {
                    var exit_identifier = outcome.Substring(6);

                    if (exit_identifier == "wilderness")
                    {
                        _controller.GoToWilderness();
                    }
                    else
                    {
                        // We go directly from one site to another, such as from a street into a building
                        var payload = new ExplorationPayload
                        {
                            SiteId = exit_identifier
                        };

                        var exit_identifier_components = exit_identifier.Split("_");

                        //if (exit_identifier_components.Length == 4)
                        //{
                        //    // Use the start position encoded in the exit identifier
                        //    var x = double.Parse(exit_identifier_components[1], CultureInfo.InvariantCulture);
                        //    var y = double.Parse(exit_identifier_components[2], CultureInfo.InvariantCulture);
                        //    var orientation = double.Parse(exit_identifier_components[3], CultureInfo.InvariantCulture);

                        //    _controller.ApplicationData.ExplorationPosition = new Vector2D(x, y);
                        //    _controller.ApplicationData.ExplorationOrientation = orientation;
                        //}
                        //else
                        //{
                        //    // Make sure to use the default start position of the site
                        //    _controller.ApplicationData.ExplorationPosition = null;
                        //    _controller.ApplicationData.ExplorationOrientation = null;
                        //}

                        _controller.GoToNextApplicationState(payload);
                    }
                }
                else if (outcome.Length >= 6 && outcome.Substring(0, 6) == "battle")
                {
                    string battleId;
                    string? entranceId = null;

                    var indexOfFirstColon = outcome.IndexOf(':');

                    if (outcome.Contains("EntranceId"))
                    {
                        var indexOfEntranceId = outcome.IndexOf("EntranceId");
                        entranceId = outcome.Substring(indexOfEntranceId + 12);
                        battleId = outcome.Substring(indexOfFirstColon + 2, indexOfEntranceId - 4 - indexOfFirstColon);
                    }
                    else
                    {
                        battleId = outcome.Substring(indexOfFirstColon + 2);
                    }

                    var payload = new BattlePayload
                    {
                        BattleId = battleId,
                        EntranceId = entranceId,
                        PayloadForNextStateInCasePartyWins = new ExplorationPayload { SiteId = _controller.ApplicationData.CurrentSiteId }
                    };

                    _controller.GoToNextApplicationState(payload);
                }
                else
                {
                    throw new InvalidDataException($"Invalid outcome after animation: {outcome}");
                }
            };
        }

        public override TempleViewModel Init(
            ApplicationStatePayload payload)
        {
            var explorationPayload = payload as ExplorationPayload
                                     ?? throw new ArgumentException("Payload is not of type ExplorationPayload", nameof(payload));

            var siteIdComponents = explorationPayload.SiteId.Split("_");
            string siteId;

            Point2D? startPosition = null;
            double? startOrientation = null;

            if (siteIdComponents.Length == 4)
            {
                siteId = siteIdComponents[0];

                var x = double.Parse(siteIdComponents[1], CultureInfo.InvariantCulture);
                var y = double.Parse(siteIdComponents[2], CultureInfo.InvariantCulture);

                startPosition = new Point2D(x, y);
                startOrientation = double.Parse(siteIdComponents[3], CultureInfo.InvariantCulture);
            }
            else
            {
                siteId = explorationPayload.SiteId;
            }

            _controller.ApplicationData.CurrentSiteId = siteId;

            var siteData = _siteDataFactory.GenerateSiteData(siteId);

            // Store the location info
            _locationInfoDictionary = siteData.LocationInfo;

            // Exclude site components having a unfulfilled game condition
            var filteredSiteData = new SiteData
            {
                SiteComponents = siteData.SiteComponents
                    .Where(_ => _.ConditionForPresence == null ||
                                _.ConditionForPresence.Evaluate(_gameQueryService))
                    .ToList(),
            };

            if (startPosition != null &&
                startOrientation.HasValue)
            {
                filteredSiteData.StartPosition = startPosition;
                filteredSiteData.StartOrientation = startOrientation.Value;
            }
            else
            {
                filteredSiteData.StartPosition = siteData.StartPosition;
                filteredSiteData.StartOrientation = siteData.StartOrientation;
            }

            _scene2D = ExplorationSceneFactory.GenerateScene(
                filteredSiteData,
                _controller,
                _gameQueryService);

            _doorGeometryCache.Clear();
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

            scene.InitialState.BodyStates.ForEach(bs =>
            {
                switch (bs.Body)
                {
                    case BodyDoor bodyDoor:
                        var bodyStateDoor = bs as BodyStateDoor;

                        var doorCenter = new Vector2D(
                            (bodyDoor.Point1.X + bodyDoor.Point2.X) / 2,
                            (bodyDoor.Point1.Y + bodyDoor.Point2.Y) / 2);

                        var doorAsVector = new Vector2D(
                            bodyDoor.Point2.X - bodyDoor.Point1.X,
                            bodyDoor.Point2.Y - bodyDoor.Point1.Y);

                        var polarAngle = Math.Atan2(-doorAsVector.Y, doorAsVector.X);
                        var doorOrientation = (polarAngle + Math.PI) * 180 / Math.PI;

                        var radius = 0.5;
                        var orientation = 0;

                        staticGeometryObjects.Add(new Circle2D_NPC(
                            new Point2D(doorCenter.X, doorCenter.Y),
                            radius,
                            "arc",
                            doorOrientation,
                            false));

                        break;
                }
            });

            scene.Boundaries.ForEach(boundary =>
            {
                if (!boundary.Visible) return;

                switch (boundary)
                {
                    case HorizontalLineSegment horizontalLineSegment:
                        staticGeometryObjects.Add(new LineSegment2D(
                            new Point2D(horizontalLineSegment.X0, horizontalLineSegment.Y),
                            new Point2D(horizontalLineSegment.X1, horizontalLineSegment.Y)));
                        break;
                    case VerticalLineSegment verticalLineSegment:
                        staticGeometryObjects.Add(new LineSegment2D(
                            new Point2D(verticalLineSegment.X, verticalLineSegment.Y0),
                            new Point2D(verticalLineSegment.X, verticalLineSegment.Y1)));
                        break;
                    case Boundaries.Trigger trigger:
                        staticGeometryObjects.Add(new LineSegment2D_Trigger(
                            new Point2D(trigger.Point1.X, trigger.Point1.Y),
                            new Point2D(trigger.Point2.X, trigger.Point2.Y)));
                        break;
                    case Craft.Simulation.Boundaries.LineSegment lineSegment:
                        staticGeometryObjects.Add(new LineSegment2D(
                            new Point2D(lineSegment.Point1.X, lineSegment.Point1.Y),
                            new Point2D(lineSegment.Point2.X, lineSegment.Point2.Y)));
                        break;
                    case BoundaryPoint boundaryPoint:
                        staticGeometryObjects.Add(
                            new Point2D(boundaryPoint.Point.X, boundaryPoint.Point.Y));
                        break;
                    case Boundaries.NPC npc:
                        staticGeometryObjects.Add(new Circle2D_NPC(
                            new Point2D(npc.Center.X, npc.Center.Y),
                            npc.Radius,
                            npc.ModelId,
                            npc.Orientation,
                            true));
                        break;
                    case Boundaries.Cylinder cylinder:
                        staticGeometryObjects.Add(new Circle2D_Cylinder(
                            new Point2D(cylinder.Center.X, cylinder.Center.Y),
                            cylinder.Radius,
                            cylinder.Length));
                        break;
                    case CircularBoundary circularBoundary:
                        staticGeometryObjects.Add(new Circle2D(
                            new Point2D(circularBoundary.Center.X, circularBoundary.Center.Y),
                            circularBoundary.Radius));
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

            var boundingBoxes = staticGeometryObjects.Select(geometryObject =>
            {
                return geometryObject switch
                {
                    Point2D point => point.ComputeBoundingBox(),
                    LineSegment2D lineSegment => lineSegment.ComputeBoundingBox(),
                    Circle2D circle => circle.ComputeBoundingBox(),
                    _ => throw new InvalidOperationException(),
                };
            });

            if (boundingBoxes.Any())
            {
                _geometryDataStore = new GeometryDataStore(
                    new Craft.DataStructures.Geometry.BoundingBox(
                        boundingBoxes.Min(b => b.MinX),
                        boundingBoxes.Max(b => b.MaxX),
                        boundingBoxes.Min(b => b.MinY),
                        boundingBoxes.Max(b => b.MaxY)));

                staticGeometryObjects.ForEach(_geometryDataStore.AddGeometricObject);
            }
        }

        private void StartAnimation(
            Scene scene)
        {
            Engine.EngineCore.Scene = scene;
            Engine.EngineCore.SpawnNewThread();
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
            var isFirstState = _currentState == null;
            _currentState = e.State;

            var bodyStateOfProtagonist = _currentState.BodyStates.First() as BodyStateClassic;

            var position = new Vector2D(
                bodyStateOfProtagonist.Position.X,
                -bodyStateOfProtagonist.Position.Y);
                
            var orientation = bodyStateOfProtagonist.Orientation;

            _controller.ApplicationData.ExplorationPosition = position;
            _controller.ApplicationData.ExplorationOrientation = 90 + orientation * 180.0 / Math.PI;

            CameraPosition = new Point3D(
                position.X,
                position.Y,
                1.7); // Eye height in meters

            LookDirection = new Vector3D(Math.Cos(orientation), Math.Sin(orientation), 0);

            DirectionalLight = LookDirection + new Vector3D(0, -0.5, 0);
            PlayerLightPosition = CameraPosition + LookDirection * 3 + new Vector3D(0, -1, 0);

            UpdateDynamicGeometricObjects();

            if (_scene2D.ViewMode == SceneViewMode.FocusOnFirstBody)
            {
                UpdateFocus(_currentState.BodyStates.First().Position);
            }

            // Door meshes need an engine state, which is not available during Init.
            // Build them even if the initial focus does not expand the world window.
            if (isFirstState)
            {
                UpdateStaticGeometricObjects();
            }
        }

        // Denne kaldes ikke så tit - kun, når extended world window opdateres
        private void UpdateStaticGeometricObjects()
        {
            GeometryViewModel.ClearLayer(false);
            DoorRotationViewModelDictionary.Clear();

            if (_geometryDataStore != null)
            {
                // This may include both visible and invisible objects. Here, we are only interested in the visible ones
                var geometricObjects =
                    _geometryDataStore.Query(GeometryViewModel.WorldWindowExpanded);

                var visibleGeometricObjects = new List<object>();

                foreach (var geometricObject in geometricObjects)
                {
                    if (geometricObject is Circle2D_NPC circle2D_NPC && !circle2D_NPC.Visible)
                    {
                        continue;
                    }

                    visibleGeometricObjects.Add(geometricObject);
                }

                // Update the static part of the 2D scene
                GeometryViewModel.AddStaticGeometryLayer(
                    visibleGeometricObjects);

                // Update the static part of the 3D scene
                Scene3DStatic = ((WpfSiteModel)_siteRenderer.Build(geometricObjects)).Model3D;
            }

            // Hent alle døre (Todo: nøjes med dem, der er tæt på spilleren)
            if (_currentState != null)
            {
                var model3DGroup = new Model3DGroup();

                var playerPosition = _currentState.BodyStates.First().Position;

                _currentState.BodyStates.ForEach(bs =>
                {
                    switch (bs.Body)
                    {
                        case BodyDoor bodyDoor:

                            if (bodyDoor.Point1.SquaredDistanceTo(playerPosition) > 100.0)
                            {
                                return;
                            }

                            var bodyStateDoor = bs as BodyStateDoor;
                            
                            var angle = (bodyStateDoor.PercentageOpen) * 90 / 100;

                            if (bodyStateDoor.OpenClockWise)
                            {
                                angle *= -1;
                            }

                            var axisAngleRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
                            var doorOpeningRotation = new RotateTransform3D(axisAngleRotation);

                            var doorRotationViewModel = new DoorRotationViewModel { RotationAngle = angle };
                            DoorRotationViewModelDictionary[bodyDoor.Id] = doorRotationViewModel;

                            BindingOperations.SetBinding(
                                axisAngleRotation,
                                AxisAngleRotation3D.AngleProperty,
                                new Binding(nameof(DoorRotationViewModel.RotationAngle))
                                {
                                    Source = doorRotationViewModel
                                });

                            var doorCenter = new Vector2D(
                                (bodyDoor.Point1.X + bodyDoor.Point2.X) / 2,
                                (bodyDoor.Point1.Y + bodyDoor.Point2.Y) / 2);

                            var doorAsVector = new Vector2D(
                                bodyDoor.Point2.X - bodyDoor.Point1.X,
                                bodyDoor.Point2.Y - bodyDoor.Point1.Y);

                            var polarAngle = Math.Atan2(-doorAsVector.Y, doorAsVector.X);
                            var doorOrientation = (polarAngle + Math.PI) * 180 / Math.PI;

                            var doorWidth = doorAsVector.Length;

                            var transform3DGroup = new Transform3DGroup();

                            // Transformer den i henhold til, hvor åben den er
                            transform3DGroup.Children.Add(new TranslateTransform3D(-doorWidth / 2, 0, 0));
                            transform3DGroup.Children.Add(doorOpeningRotation);
                            transform3DGroup.Children.Add(new TranslateTransform3D(doorWidth / 2, 0, 0));

                            // Placer den i scenen
                            transform3DGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), doorOrientation)));
                            transform3DGroup.Children.Add(new TranslateTransform3D(doorCenter.X, -doorCenter.Y, 0));
                            var meshDoor = MeshBuilder.CreateMesh("door");

                            var modelDoor = new GeometryModel3D
                            {
                                Geometry = meshDoor,
                                Material = _materialDoor,
                                BackMaterial = _materialDoor
                            };

                            modelDoor.Transform = transform3DGroup;
                            model3DGroup.Children.Add(modelDoor);

                            break;
                    }
                });

                Scene3DDynamic = model3DGroup;
            }
        }

        private void UpdateDynamicGeometricObjects()
        {
            var geometryIndex = 0;
            var geometryChanged = false;
            _doorGeometryCache.BeginFrame();

            foreach (var bs in _currentState.BodyStates)
            {
                object geometry;
                switch (bs.Body)
                {
                    case CircularBody circularBody:
                        var previousCircle = geometryIndex < _dynamicGeometricObjects.Count
                            ? _dynamicGeometricObjects[geometryIndex] as Circle2D
                            : null;
                        geometry = previousCircle != null &&
                                   previousCircle.Center.X == bs.Position.X &&
                                   previousCircle.Center.Y == bs.Position.Y &&
                                   previousCircle.Radius == circularBody.Radius
                            ? previousCircle
                            : new Circle2D(new Point2D(bs.Position.X, bs.Position.Y), circularBody.Radius);
                        break;
                    case BodyDoor bodyDoor:
                        var bodyStateDoor = (BodyStateDoor)bs;
                        geometry = _doorGeometryCache.GetGeometry(bodyStateDoor);

                        if (DoorRotationViewModelDictionary.TryGetValue(bodyDoor.Id, out var rotation))
                        {
                            var angle = bodyStateDoor.PercentageOpen * 90 / 100;
                            rotation.RotationAngle = bodyStateDoor.OpenClockWise ? -angle : angle;
                        }
                        break;
                    default:
                        continue;
                }

                if (geometryIndex == _dynamicGeometricObjects.Count)
                {
                    _dynamicGeometricObjects.Add(geometry);
                    geometryChanged = true;
                }
                else if (!ReferenceEquals(_dynamicGeometricObjects[geometryIndex], geometry))
                {
                    _dynamicGeometricObjects[geometryIndex] = geometry;
                    geometryChanged = true;
                }
                geometryIndex++;
            }

            if (geometryIndex < _dynamicGeometricObjects.Count)
            {
                _dynamicGeometricObjects.RemoveRange(geometryIndex, _dynamicGeometricObjects.Count - geometryIndex);
                geometryChanged = true;
            }

            _doorGeometryCache.EndFrame();

            // Notify the canvas after updating the retained layer. Replacing the entire
            // dynamic layer through Craft's helper also clears and re-adds static layers.
            if (geometryChanged)
            {
                var layerIndex = GeometryViewModel.GeometryLayers.IndexOf(_dynamicGeometryLayer);
                GeometryViewModel.GeometryLayers[layerIndex] = _dynamicGeometryLayer;
            }
        }

        private void UpdateFocus(
            Vector2D focus)
        {
            GeometryViewModel.RequestedWorldFocus = new WorldFocusRequest
            {
                WorldPoint = new Point(focus.X, focus.Y),
                ViewportRatio = new Size(0.5, 0.5),
                Scaling = new Size(0.015, 0.015) // (Ordinary)
                //Scaling = new Size(0.003, 0.003) // (Zoom in x 5)
                //Scaling = new Size(0.0015, 0.0015) // (Zoom in x 10)
                //Scaling = new Size(0.15, 0.15) // (Zoom out x 10)
            };
        }
    }
}
