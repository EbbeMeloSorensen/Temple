using Craft.Logging;
using Craft.Utils;
using Temple.Application.Core;
using Temple.Application.State;
using Temple.Domain.Entities.DD;
using Temple.ViewModel.DD.Battle;
using Temple.ViewModel.DD.Battle.BusinessLogic;
using Temple.ViewModel.DD.BusinessLogic.Complex;

namespace Temple.ViewModel.DD;

public class BattleViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private ApplicationStatePayload _payloadForNextState;

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
                _controller.GoToNextApplicationState(_payloadForNextState);
            }
        };
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var battlePayload = payload as BattlePayload
                            ?? throw new ArgumentException("Payload is not of type BattlePayload", nameof(payload));

        var scene = GetSceneFirstBattle();
        _payloadForNextState = battlePayload.PayloadForNextStateInCasePartyWins;
        ActOutSceneViewModel.InitializeScene(scene);
        ActOutSceneViewModel.StartBattleCommand.ExecuteAsync();

        return this;
    }

    private Scene GetSceneFirstBattle()
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

        var scene = new Domain.Entities.DD.Scene("DummyScene", 8, 7);
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 3));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 4));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 6));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 6));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 5));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 6));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 5));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 2));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 1));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 2));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 1));
        scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 5, 3);
        scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 1, 4);

        return scene;
    }

    private Scene GetSceneFinalBattle()
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

        var scene = new Domain.Entities.DD.Scene("DummyScene", 11, 12);
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 5, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 6, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 0));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 1));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 1));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 2));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 2));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 3));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 3));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 4));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 4));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 5));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 5));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 6));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 6));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 7));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 8));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 8));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 9));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 0, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 2, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 3, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 4, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 7, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 8, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 9, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 10, 10));
        scene.AddObstacle(new Obstacle(ObstacleType.Wall, 11, 10));
        scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 6, 9);
        scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 4, 1);
        scene.AddCreature(new Creature(goblinArcher, true) { IsAutomatic = true }, 7, 1);

        return scene;
    }
}