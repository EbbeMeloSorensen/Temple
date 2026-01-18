using Temple.Domain.Entities.DD.Quests;

namespace Temple.ViewModel.DD.Quests;

public class QuestStateChangedEventArgs : EventArgs
{
    public string QuestId { get; }
    public QuestState QuestState { get; }

    public QuestStateChangedEventArgs(
        string questId,
        QuestState questState)
    {
        QuestId = questId;
        QuestState = questState;
    }
}