using Temple.Application.DD;
using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Interfaces;

public interface IQuestStatusReadModel
{
    IEnumerable<string> Quests { get; }

    event EventHandler<QuestStatusChangedEventArgs>? QuestStatusChanged;

    void Initialize(
        IReadOnlyCollection<Quest> quests);

    QuestStatus GetQuestStatus(
        string questId);
}