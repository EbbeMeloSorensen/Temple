namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class FactEstablishedEvent : IGameEvent
{
    public string FactId { get; }

    public FactEstablishedEvent(string factId)
    {
        FactId = factId;
    }
}