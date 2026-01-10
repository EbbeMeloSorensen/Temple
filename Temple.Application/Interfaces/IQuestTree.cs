namespace Temple.Application.Interfaces;

public interface IQuestTree
{
    int GetQuestCount();

    Quest GetQuestById(int id);

    IEnumerable<Quest> GetAllQuests();
}