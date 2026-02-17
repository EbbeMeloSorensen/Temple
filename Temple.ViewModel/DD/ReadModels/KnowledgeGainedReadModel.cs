using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.ViewModel.DD.ReadModels;

public class KnowledgeGainedReadModel : IKnowledgeGainedReader
{
    private readonly HashSet<string> _knowledgeGained = new HashSet<string>();

    public KnowledgeGainedReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<KnowledgeGainedEvent>(HandleKnowledgeGained);
    }

    private void HandleKnowledgeGained(
        KnowledgeGainedEvent e)
    {
        _knowledgeGained.Add(e.KnowledgeId);
    }

    public bool IsKnowledgeGained(
        string knowledgeId)
    {
        return _knowledgeGained.Contains(knowledgeId);
    }
}