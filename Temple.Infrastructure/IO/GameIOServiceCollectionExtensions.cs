using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.IO;

public static class GameIOServiceCollectionExtensions
{
    public static IServiceCollection AddGameIO(this IServiceCollection services, string saveDirectory)
    {
        services.AddSingleton<IGameIOHandler, GameIOHandler>();
        services.AddSingleton<IGameSaveStore>(_ => new JsonGameSaveStore(saveDirectory));

        return services;
    }
}
