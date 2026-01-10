namespace Temple.Application.Interfaces;

public interface IQuestManager
{
    IEnumerable<Quest> GetAllQuests();
}