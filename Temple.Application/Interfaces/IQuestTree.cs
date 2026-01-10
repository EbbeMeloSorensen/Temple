namespace Temple.Application.Interfaces;

public interface IQuestTree
{
    IEnumerable<Quest> GetAllQuests();
}