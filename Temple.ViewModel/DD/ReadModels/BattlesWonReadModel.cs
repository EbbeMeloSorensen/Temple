using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.ViewModel.DD.ReadModels;

public class BattlesWonReadModel : IBattlesWonReader
{
    private readonly HashSet<string> _battlesWon = new HashSet<string>();


    public BattlesWonReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<BattleWonEvent>(HandleBattleWon);
    }

    public bool BattleWon(
        string battleId)
    {
        return _battlesWon.Contains(battleId);
    }

    private void HandleBattleWon(
        BattleWonEvent e)
    {
        _battlesWon.Add(e.BattleId);
    }
}