using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Interfaces;

public interface IQuestStateReadModel
{
    QuestState GetQuestState(
        string questId);
}