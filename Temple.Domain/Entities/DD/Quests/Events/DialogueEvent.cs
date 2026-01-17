namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class DialogueEvent : IGameEvent
{
    public string NpcId { get; }

    public DialogueEvent(string npcId)
    {
        NpcId = npcId;
    }
}