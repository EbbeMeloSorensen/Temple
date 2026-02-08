namespace Temple.Application.Interfaces;

public interface IKnowledgeGainedReadModel
{
    public IEnumerable<string> KnowledgeGained { get; }
}