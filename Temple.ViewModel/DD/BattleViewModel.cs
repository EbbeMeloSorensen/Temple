using Craft.Logging;
using Craft.Utils;
using GalaSoft.MvvmLight;
using Temple.Application.Core;
using Temple.Domain.Entities.DD;
using Temple.ViewModel.DD.Battle;
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
    }
}