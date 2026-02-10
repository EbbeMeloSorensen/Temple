namespace Temple.Infrastructure.Dialogues.GameEventTriggers;

public class FactEstablishedEventTrigger : IGameEventTrigger
{
    public string FactId { get; set; }

    public FactEstablishedEventTrigger(
        string factId)
    {
        FactId = factId;
    }

    public override string ToString()
    {
        return $"Fact established: {FactId}";
    }
}