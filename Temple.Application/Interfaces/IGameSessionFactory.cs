using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IGameSessionFactory
{
    GameSession CreateNew();
}
