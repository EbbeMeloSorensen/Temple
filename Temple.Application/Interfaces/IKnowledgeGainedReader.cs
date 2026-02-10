namespace Temple.Application.Interfaces;

public interface IKnowledgeGainedReader
{
    public IEnumerable<string> KnowledgeGained { get; }
}