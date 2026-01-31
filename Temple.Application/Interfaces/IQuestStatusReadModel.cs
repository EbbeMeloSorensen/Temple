using Temple.Application.DD;

namespace Temple.Application.Interfaces;

public interface IQuestStatusReadModel
{
    IEnumerable<string> Quests { get; }

    event EventHandler<QuestStatusChangedEventArgs>? QuestStatusChanged;

    QuestStatus GetQuestStatus(
        string questId);
}