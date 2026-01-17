using Temple.Domain.Entities.DD.Quests;

namespace Temple.ViewModel.DD.Quests;

public class QuestStatusChangedEventArgs : EventArgs
{
    public string QuestId { get; }
    public QuestState QuestState { get; }

    public QuestStatusChangedEventArgs(
        string questId,
        QuestState questState)
    {
        QuestId = questId;
        QuestState = questState;
    }
}