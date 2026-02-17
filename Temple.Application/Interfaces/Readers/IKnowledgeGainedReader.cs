namespace Temple.Application.Interfaces.Readers;

public interface IKnowledgeGainedReader
{
    public bool IsKnowledgeGained(
        string knowledgeId);
}