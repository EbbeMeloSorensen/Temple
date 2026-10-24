using Temple.Application.Saves;

namespace Temple.Application.Interfaces;

public interface IGameSaveStore
{
    Task WriteAsync(int slot, GameSave save, CancellationToken cancellationToken = default);
    Task<GameSave> ReadAsync(int slot, CancellationToken cancellationToken = default);
}
