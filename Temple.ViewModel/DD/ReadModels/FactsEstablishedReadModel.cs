using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.ViewModel.DD.ReadModels;

public class FactsEstablishedReadModel : IFactsEstablishedReader
{
    private readonly HashSet<string> _factsEstablished = new HashSet<string>();

    public FactsEstablishedReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<FactEstablishedEvent>(HandleFactEstablished);
    }

    public bool FactEstablished(string factId)
    {
        return _factsEstablished.Contains(factId);
    }

    private void HandleFactEstablished(
        FactEstablishedEvent e)
    {
        _factsEstablished.Add(e.FactId);
    }
}