namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class DialogueCompletedEvent : IGameEvent
{
    public string NpcId { get; }

    public DialogueCompletedEvent(string npcId)
    {
        NpcId = npcId;
    }
}