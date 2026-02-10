using Temple.Application.DD;
using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Interfaces.Readers;

public interface IQuestStatusReader
{
    IEnumerable<string> QuestIds { get; }

    event EventHandler<QuestStatusChangedEventArgs>? QuestStatusChanged;

    void Initialize(
        IReadOnlyCollection<Quest> quests);

    QuestStatus GetQuestStatus(
        string questId);
}