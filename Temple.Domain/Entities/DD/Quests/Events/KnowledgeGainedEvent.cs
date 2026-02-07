namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class KnowledgeGainedEvent : IGameEvent
{
    public string KnowledgeId { get; }

    public KnowledgeGainedEvent(string knowledgeId)
    {
        KnowledgeId = knowledgeId;
    }
}