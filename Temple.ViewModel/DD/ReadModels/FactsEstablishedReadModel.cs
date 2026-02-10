using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.ViewModel.DD.ReadModels;

public class FactsEstablishedReadModel : IFactsEstablishedReader
{
    private readonly HashSet<string> _factsEstablished = new HashSet<string>();

    public IEnumerable<string> FactsEstablished => _factsEstablished;

    public FactsEstablishedReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<FactEstablishedEvent>(HandleFactEstablished);
    }

    private void HandleFactEstablished(
        FactEstablishedEvent e)
    {
        _factsEstablished.Add(e.FactId);
    }
}