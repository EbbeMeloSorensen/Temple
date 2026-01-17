namespace Temple.Domain.Entities.DD.Quests;

public sealed class QuestStatus
{
    public string QuestId { get; }
    public QuestState State { get; private set; }

    public QuestStatus(
        string questId,
        QuestState state)
    {
        QuestId = questId;
        State = state;
    }

    public void UpdateState(
        QuestState newState)
    {
        State = newState;
    }
}
