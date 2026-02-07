namespace Temple.Infrastructure.Dialogues;

public class QuestDiscoveredEventTrigger : IGameEventTrigger
{
    public string QuestId { get; set; }
    public QuestDiscoveredEventTrigger(
        string questId)
    {
        QuestId = questId;
    }

    public override string ToString()
    {
        return $"Quest discovered: {QuestId}";
    }
}