using Craft.Logging;
using Craft.Utils;
using GalaSoft.MvvmLight;
using Temple.Application.Core;
using Temple.Domain.Entities.DD;
using Temple.ViewModel.DD.BusinessLogic;
using Temple.ViewModel.DD.BusinessLogic.Complex;

namespace Temple.ViewModel.DD;

public class BattleViewModel : ViewModelBase
{
    private readonly ApplicationController _controller;

    public BoardViewModelBase BoardViewModel { get; }

    public ActOutSceneViewModelBase ActOutSceneViewModel { get; }

    public BattleViewModel(
        ApplicationController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        ILogger logger = null;
        var engine = new ComplexEngine(logger);
        engine.BoardTileMode = BoardTileMode.Square;

        var tileCenterSpacing = 80;
        var obstacleDiameter = 80;
        var creatureDiameter = 75;
        var projectileDiameter = 75;

        var dummyScene = new ObservableObject<Scene>();

        BoardViewModel = 
            new BoardViewModel(
                engine: engine,
                tileCenterSpacing,
                obstacleDiameter,
                creatureDiameter,
                projectileDiameter,
                dummyScene);

        ActOutSceneViewModel = new ActOutSceneViewModelComplexEngine(
            engine,
            BoardViewModel,
            dummyScene,
            logger);

        ActOutSceneViewModel.BattleEnded += (s, e) =>
        {
            if (e.BattleResult == BattleResult.Defeat)
            {
                _controller.GoToDefeat();
            }
            else
            {
                _controller.GoToVictory();
            }
        };

        var knight = new CreatureType("Knight",
            maxHitPoints: 8, //20,
            armorClass: 3,
            thaco: 12,
            initiativeModifier: 0,
            movement: 4,
            attacks:  new List<Attack>
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

        dummyScene.Object = scene;
    }
}