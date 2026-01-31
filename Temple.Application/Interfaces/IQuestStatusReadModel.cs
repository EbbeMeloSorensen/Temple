using Temple.Application.DD;

namespace Temple.Application.Interfaces;

public interface IQuestStatusReadModel
{
    event EventHandler<QuestStatusChangedEventArgs>? QuestStatusChanged;

    QuestStatus GetQuestStatus(
        string questId);
}