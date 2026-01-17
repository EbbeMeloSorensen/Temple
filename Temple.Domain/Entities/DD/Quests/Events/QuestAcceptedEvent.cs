namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class QuestAcceptedEvent : IGameEvent
{
    public string QuestId { get; }

    public QuestAcceptedEvent(string questId)
    {
        QuestId = questId;
    }
}