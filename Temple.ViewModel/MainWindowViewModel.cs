using Craft.Math;
using Craft.Simulation;
using Craft.Simulation.Bodies;
using Craft.Simulation.BodyStates;
using Craft.Simulation.Boundaries;
using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using MediatR;
using Temple.Application.Core;
using Temple.Domain.Entities.DD;
using Temple.ViewModel.DD;
using Temple.ViewModel.PR;
using Temple.ViewModel.Smurfs;

namespace Temple.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _applicationDialogService;
        private readonly ApplicationController _controller;
        private string _currentApplicationStateAsText;
        private InterludeViewModel _interludeViewModel;
        private BattleViewModel _battleViewModel;
        private ExploreAreaViewModel _exploreAreaViewModel;
        private DefeatViewModel _defeatViewModel;
        private VictoryViewModel _victoryViewModel;
        private MainWindowViewModel_Smurfs _mainWindowViewModel_Smurfs;
        private MainWindowViewModel_PR _mainWindowViewModel_PR;
        private object _currentViewModel;

        public Action? ShutdownAction { get; set; }

        public HomeViewModel HomeViewModel { get; }

        public InterludeViewModel InterludeViewModel
        {
            get
            {
                return _interludeViewModel ??= new InterludeViewModel(_controller);
            }
        }

        public BattleViewModel BattleViewModel
        {
            get
            {
                return _battleViewModel ??= new BattleViewModel(_controller);
            }
        }

        public ExploreAreaViewModel ExploreAreaViewModel
        {
            get
            {
                return _exploreAreaViewModel ??= new ExploreAreaViewModel(_controller);
            }
        }

        public DefeatViewModel DefeatViewModel
        {
            get
            {
                return _defeatViewModel ??= new DefeatViewModel(_controller);
            }
        }

        public VictoryViewModel VictoryViewModel
        {
            get
            {
                return _victoryViewModel ??= new VictoryViewModel(_controller);
            }
        }

        public MainWindowViewModel_Smurfs MainWindowViewModel_Smurfs
        {
            get
            {
                return _mainWindowViewModel_Smurfs ??= new MainWindowViewModel_Smurfs(_mediator, _controller);
            }
        }

        public MainWindowViewModel_PR MainWindowViewModel_PR
        {
            get
            {
                return _mainWindowViewModel_PR ??= new MainWindowViewModel_PR(_mediator, _applicationDialogService, _controller);
            }
        }

        public string CurrentApplicationStateAsText
        {
            get => _currentApplicationStateAsText;
            private set => Set(ref _currentApplicationStateAsText, value);
        }

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(
            IMediator mediator,
            IDialogService applicationDialogService,
            ApplicationController controller)
        {
            _mediator = mediator;
            _applicationDialogService = applicationDialogService;
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            HomeViewModel = new HomeViewModel(_controller);

            CurrentApplicationStateAsText = _controller.CurrentApplicationState.Type.ToString();

            _controller.SceneChanged += (newScene) =>
            {
                CurrentApplicationStateAsText = newScene.Type.ToString();

                switch (CurrentApplicationStateAsText)
                {
                    case "MainMenu":
                        CurrentViewModel = HomeViewModel;
                        break;
                    case "ShuttingDown":
                        ShutdownAction?.Invoke();
                        break;
                    case "SmurfManagement":
                        CurrentViewModel = MainWindowViewModel_Smurfs;
                        break;
                    case "PeopleManagement":
                        CurrentViewModel = MainWindowViewModel_PR;
                        break;
                    case "Intro":
                        InterludeViewModel.Text = "Din lille gruppe af eventyrere har været ude og fange mosegrise og er nu på vej hjem til byen for at sælge dem på markedet. Men sjovt nok bliver i overfaldet af banditter på vejen. Gør klar til kamp!";
                        CurrentViewModel = InterludeViewModel;
                        break;
                    case "ExploreArea_AfterFirstBattle":
                        CurrentViewModel = ExploreAreaViewModel;
                        ExploreAreaViewModel.StartAnimation(GenerateScene());
                        break;
                    case "Battle_First":
                        BattleViewModel.ActOutSceneViewModel.InitializeScene(GetSceneFirstBattle());
                        CurrentViewModel = BattleViewModel;
                        // Start the battle automatically, so the user doesn't need to click the start button
                        BattleViewModel.ActOutSceneViewModel.StartBattleCommand.ExecuteAsync();
                        break;
                    case "Battle_Final":
                        BattleViewModel.ActOutSceneViewModel.InitializeScene(GetSceneFinalBattle());
                        CurrentViewModel = BattleViewModel;
                        // Start the battle automatically, so the user doesn't need to click the start button
                        BattleViewModel.ActOutSceneViewModel.StartBattleCommand.ExecuteAsync();
                        break;
                    case "Defeat":
                        CurrentViewModel = DefeatViewModel;
                        break;
                    case "Victory":
                        CurrentViewModel = VictoryViewModel;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid operation");
                }
            };

            CurrentViewModel = new HomeViewModel(_controller);
        }

        private Domain.Entities.DD.Scene GetSceneFirstBattle()
        {
            var knight = new CreatureType("Knight",
                maxHitPoints: 8, //20,
                armorClass: 3,
                thaco: 1, //12,
                initiativeModifier: 0,
                movement: 4,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10),
                    new MeleeAttack("Longsword", 100),// 10)
                });

            var goblin = new CreatureType(
                name: "Goblin",
                maxHitPoints: 12,
                armorClass: 5,
                thaco: 20,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Short sword", 6)
                });

            var archer = new CreatureType(
                name: "Archer",
                maxHitPoints: 12,
                armorClass: 6,
                thaco: 14,
                initiativeModifier: 10,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 5)
                });

            var goblinArcher = new CreatureType(
                name: "Goblin Archer",
                maxHitPoints: 20,
                armorClass: 7,
                thaco: 13,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 4)
                });

            var scene = new Domain.Entities.DD.Scene("DummyScene", 4, 4);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 2, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 2, 2));
            scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 0, 0);
            //scene.AddCreature(new Creature(archer, false) { IsAutomatic = false }, 0, 1);
            scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 3, 2);
            //scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 3, 3);

            return scene;
        }

        private Domain.Entities.DD.Scene GetSceneFinalBattle()
        {
            var knight = new CreatureType("Knight",
                maxHitPoints: 8, //20,
                armorClass: 3,
                thaco: 12,
                initiativeModifier: 0,
                movement: 4,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Longsword", 10),
                    new MeleeAttack("Longsword", 10)
                });

            var goblin = new CreatureType(
                name: "Goblin",
                maxHitPoints: 12,
                armorClass: 5,
                thaco: 20,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new MeleeAttack("Short sword", 6)
                });

            var archer = new CreatureType(
                name: "Archer",
                maxHitPoints: 12,
                armorClass: 6,
                thaco: 14,
                initiativeModifier: 10,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 5)
                });

            var goblinArcher = new CreatureType(
                name: "Goblin Archer",
                maxHitPoints: 20,
                armorClass: 7,
                thaco: 13,
                initiativeModifier: 0,
                movement: 6,
                attacks: new List<Attack>
                {
                    new RangedAttack(
                        name: "Bow & Arrow",
                        maxDamage: 4,
                        range: 4)
                });

            var scene = new Domain.Entities.DD.Scene("DummyScene", 4, 4);
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 2));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 2, 1));
            scene.AddObstacle(new Obstacle(ObstacleType.Water, 2, 2));
            scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 0, 0);
            //scene.AddCreature(new Creature(archer, false) { IsAutomatic = false }, 0, 1);
            //scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 3, 2);
            scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 3, 2);
            scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 3, 3);

            return scene;
        }

        private Craft.Simulation.Scene GenerateScene()
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

            var scene = new Craft.Simulation.Scene(
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
                new(new Point2D(-1, 3), new Point2D(-1, 2)),
                new(new Point2D(-1, 2), new Point2D(-2, 2)),
                new(new Point2D(-2, 2), new Point2D(-2, 1)),
                new(new Point2D(-2, 1), new Point2D(-3, 1)),
                new(new Point2D(-3, 0), new Point2D(-2, 0)),
                new(new Point2D(-2, 0), new Point2D(-2, -1)),
                new(new Point2D(-2, -1), new Point2D(1, -1)),
                new(new Point2D(1, -1), new Point2D(1, 2)),
                new(new Point2D(1, 2), new Point2D(0, 2)),
                new(new Point2D(0, 2), new Point2D(0, 3)),
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


            // Add exits
            scene.AddBoundary(new LineSegment(new Vector2D(-3, -1), new Vector2D(-3, 0), "A"));
            scene.AddBoundary(new LineSegment(new Vector2D(0, -3), new Vector2D(-1, -3), "B"));

            scene.PostPropagationCallBack = (propagatedState, boundaryCollisionReports, bodyCollisionReports) =>
            {
                var response = new PostPropagationResponse();

                if (boundaryCollisionReports.Any())
                {
                    var boundary = boundaryCollisionReports.First().Boundary;

                    response.Outcome = boundary.Tag;
                    response.IndexOfLastState = propagatedState.Index;
                }

                return response;
            };


            return scene;
        }
    }
}
