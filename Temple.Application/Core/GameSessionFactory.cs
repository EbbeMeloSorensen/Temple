using Temple.Application.Interfaces;

namespace Temple.Application.Core;

public sealed class GameSessionFactory(IGameIOHandler gameIOHandler) : IGameSessionFactory
{
    public GameSession CreateNew() => new(
        gameIOHandler.ReadSiteIdsFromDirectory("DD//Assets//SiteData").ToList(),
        gameIOHandler.ReadQuestListFromFile("DD//Assets//Quests//quests.json").ToList());
}
