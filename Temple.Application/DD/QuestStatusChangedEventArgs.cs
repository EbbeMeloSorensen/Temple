using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.DD;

public class QuestStatusChangedEventArgs : EventArgs
{
    public string QuestId { get; }
    public QuestStatus QuestStatus { get; }

    public QuestStatusChangedEventArgs(
        string questId,
        QuestStatus questStatus)
    {
        QuestId = questId;
        QuestStatus = questStatus;
    }
}