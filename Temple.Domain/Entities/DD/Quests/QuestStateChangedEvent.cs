using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests;

public sealed class QuestStateChangedEvent : IGameEvent
{
    public string QuestId { get; }
    public QuestState OldState { get; }
    public QuestState NewState { get; }

    public QuestStateChangedEvent(
        string questId,
        QuestState oldState,
        QuestState newState)
    {
        QuestId = questId;
        OldState = oldState;
        NewState = newState;
    }
}