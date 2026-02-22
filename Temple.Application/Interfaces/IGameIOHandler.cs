using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Interfaces;

public enum IOMode
{
    Read,
    Write
}

public interface IGameIOHandler
{
    public void WriteQuestsToFile(
        IEnumerable<Quest> quests,
        string fileName);

    IEnumerable<Quest> ReadQuestListFromFile(
        string fileName);
}