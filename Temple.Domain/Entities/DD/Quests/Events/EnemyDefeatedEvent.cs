namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class EnemyDefeatedEvent : IGameEvent
{
    public string EnemyId { get; }

    public EnemyDefeatedEvent(string enemyId)
    {
        EnemyId = enemyId;
    }
}