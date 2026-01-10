namespace Temple.Application.Interfaces;

public interface IQuestManager
{
    IEnumerable<Quest> GetAllQuests();

    IEnumerable<Quest> GetAvailableQuests();

    IEnumerable<Quest> GetSubsequentQuests(Quest quest);
}