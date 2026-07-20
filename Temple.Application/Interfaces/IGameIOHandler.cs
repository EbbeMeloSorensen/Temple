using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Interfaces;

public enum IOMode
{
    Read,
    Write
}

public interface IGameIOHandler
{
    IEnumerable<string> ReadSiteIdsFromDirectory(
        string directoryPath);

    public void WriteQuestsToFile(
        IEnumerable<Quest> quests,
        string fileName);

    IEnumerable<Quest> ReadQuestListFromFile(
        string fileName);
}