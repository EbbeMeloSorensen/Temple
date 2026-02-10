namespace Temple.Application.Interfaces.Readers;

public interface IKnowledgeGainedReader
{
    public IEnumerable<string> KnowledgeGained { get; }
}