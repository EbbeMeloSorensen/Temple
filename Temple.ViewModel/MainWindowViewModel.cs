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
        private string _currentState;
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

        public string CurrentState
        {
            get => _currentState;
            private set => Set(ref _currentState, value);
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

            CurrentState = _controller.CurrentState.ToString();

            _controller.StateChanged += (_, e) =>
            {
                CurrentState = e.NewState.ToString();

                switch (CurrentState)
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

        private Scene GetSceneFirstBattle()
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

            var scene = new Scene("DummyScene", 4, 4);
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

        private Scene GetSceneFinalBattle()
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

            var scene = new Scene("DummyScene", 4, 4);
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
    }
}
