using FluentValidation;
using Temple.Application.Interfaces;
using Temple.Application.Saves;

namespace Temple.Application.Core;

public sealed class GameSessionFactory(IGameIOHandler gameIOHandler, IValidator<GameSave> validator) : IGameSessionFactory
{
    public GameSession CreateNew() => Create(null);

    public GameSession Restore(GameSave save)
    {
        ArgumentNullException.ThrowIfNull(save);
        validator.ValidateAndThrow(save);
        return Create(save);
    }

    private GameSession Create(GameSave? save) => new(
        gameIOHandler.ReadSiteIdsFromDirectory("DD//Assets//SiteData").ToList(),
        gameIOHandler.ReadQuestListFromFile("DD//Assets//Quests//quests.json").ToList(), save);
}
