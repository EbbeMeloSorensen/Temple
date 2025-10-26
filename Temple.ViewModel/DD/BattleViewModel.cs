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

        var knight = new CreatureType(
            "Knight", 20, 3, 12, 0, 4, new List<Attack>
            {
                new MeleeAttack("Longsword", 10),
                new MeleeAttack("Longsword", 10)
            });

         var goblin = new CreatureType(
            "Goblin", 12, 5, 20, 0, 6, new List<Attack>
            {
                new MeleeAttack("Short sword", 6)
            });

        var scene = new Scene("DummyScene", 4, 4);
        //scene.AddObstacle(new Obstacle(ObstacleType.Wall, 1, 3));
        //scene.AddObstacle(new Obstacle(ObstacleType.Water, 1, 2));
        scene.AddCreature(new Creature(knight, false) { IsAutomatic = false }, 1, 1);
        scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 3, 1);
        //scene.AddCreature(new Creature(goblin, true) { IsAutomatic = true }, 3, 2);

        dummyScene.Object = scene;
    }
}