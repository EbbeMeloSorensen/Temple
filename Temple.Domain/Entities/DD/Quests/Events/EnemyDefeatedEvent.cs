namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class BattleWonEvent : IGameEvent
{
    public string BattleId { get; }

    public BattleWonEvent(
        string battleId)
    {
        BattleId = battleId;
    }
}