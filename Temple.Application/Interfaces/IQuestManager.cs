namespace Temple.Application.Interfaces;

public interface IQuestManager
{
    int GetQuestCount();

    Quest GetQuestById(int id);

    IEnumerable<Quest> GetAllQuests();
}