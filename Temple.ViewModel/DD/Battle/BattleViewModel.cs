using Craft.Logging;
using Temple.Application.Core;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.ViewModel.DD.Battle.BusinessLogic;
using Temple.ViewModel.DD.Battle.BusinessLogic.Complex;

namespace Temple.ViewModel.DD.Battle;

public class BattleViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private string _battleId;
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

        BoardViewModel = 
            new BoardViewModel(
                engine: engine,
                tileCenterSpacing,
                obstacleDiameter,
                creatureDiameter,
                projectileDiameter);

        ActOutSceneViewModel = new ActOutSceneViewModelComplexEngine(
            engine,
            BoardViewModel,
            logger);

        ActOutSceneViewModel.BattleEnded += (s, e) =>
        {
            if (e.BattleResult == BattleResult.Defeat)
            {
                _controller.GoToDefeat();
            }
            else
            {
                // Victory
                if (_battleId == "Final Battle")
                {
                    _controller.GoToVictory();
                }
                else
                {
                    _controller.ApplicationData.BattlesWon.Add(_battleId);
                    _controller.EventBus.Publish(new BattleWonEvent(_battleId));
                    _controller.GoToNextApplicationState(_payloadForNextState);
                }
            }
        };
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var battlePayload = payload as BattlePayload
                            ?? throw new ArgumentException("Payload is not of type BattlePayload", nameof(payload));

        _battleId = battlePayload.BattleId;
        _payloadForNextState = battlePayload.PayloadForNextStateInCasePartyWins;

        ActOutSceneViewModel.InitializeScene(BattleSceneFactory.SetupBattleScene(
            _controller.ApplicationData.Party,
            battlePayload.BattleId,
            battlePayload.EntranceId));

        ActOutSceneViewModel.StartBattleCommand.ExecuteAsync();

        return this;
    }
}