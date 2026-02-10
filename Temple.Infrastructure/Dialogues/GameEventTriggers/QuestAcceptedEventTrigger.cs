namespace Temple.Infrastructure.Dialogues.GameEventTriggers;

public class QuestAcceptedEventTrigger : IGameEventTrigger
{
    public string QuestId { get; set; }
    public QuestAcceptedEventTrigger(
        string questId)
    {
        QuestId = questId;
    }

    public override string ToString()
    {
        return $"Quest accepted: {QuestId}";
    }
}