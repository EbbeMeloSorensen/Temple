using Temple.Application.Core;
using Temple.Application.Saves;

namespace Temple.Application.Interfaces;

public interface IGameSessionFactory
{
    GameSession CreateNew();
    GameSession Restore(GameSave save);
}
