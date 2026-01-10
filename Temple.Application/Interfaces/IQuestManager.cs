namespace Temple.Application.Interfaces;

public interface IQuestManager
{
    IEnumerable<Quest> GetAllQuests();

    IEnumerable<Quest> GetAvailableAndStartedQuests();

    Quest GetQuestById(
        int questId);

    IEnumerable<Quest> GetSubsequentQuests(
        Quest quest);
}