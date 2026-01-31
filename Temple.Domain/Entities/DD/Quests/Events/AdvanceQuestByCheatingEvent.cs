namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class AdvanceQuestByCheatingEvent : IGameEvent
{
    public string QuestId { get; }

    public AdvanceQuestByCheatingEvent(
        string questId)
    {
        QuestId = questId;
    }
}