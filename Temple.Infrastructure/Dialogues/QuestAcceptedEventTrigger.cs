namespace Temple.Infrastructure.Dialogues;

public class QuestAcceptedEventTrigger : IGameEventTrigger
{
    public string QuestId { get; set; }
    public QuestAcceptedEventTrigger(
        string questId)
    {
        QuestId = questId;
    }
}