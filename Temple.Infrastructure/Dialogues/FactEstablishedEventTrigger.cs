namespace Temple.Infrastructure.Dialogues;

public class KnowledgeGainedEventTrigger : IGameEventTrigger
{
    public string KnowledgeId { get; set; }

    public KnowledgeGainedEventTrigger(
        string knowledgeId)
    {
        KnowledgeId = knowledgeId;
    }

    public override string ToString()
    {
        return $"Knowledge gained: {KnowledgeId}";
    }
}