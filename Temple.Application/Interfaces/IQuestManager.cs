namespace Temple.Application.Interfaces;

public interface IQuestManager
{
    IEnumerable<QuestOld> GetAllQuests();

    IEnumerable<QuestOld> GetAvailableAndStartedQuests();

    QuestOld GetQuestById(
        int questId);

    IEnumerable<QuestOld> GetSubsequentQuests(
        QuestOld questOld);
}