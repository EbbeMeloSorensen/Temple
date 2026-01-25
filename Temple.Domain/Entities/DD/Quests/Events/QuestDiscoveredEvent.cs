namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class QuestDiscoveredEvent : IGameEvent
{
    public string QuestId { get; }

    public QuestDiscoveredEvent(
        string questId)
    {
        QuestId = questId;
    }
}