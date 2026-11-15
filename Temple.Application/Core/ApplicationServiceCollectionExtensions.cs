using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Interfaces;
using Temple.Application.Saves;

namespace Temple.Application.Core;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(People.List.Handler).Assembly));

        services.AddValidatorsFromAssembly(typeof(People.Create.CommandValidator).Assembly,
            filter: result => result.ValidatorType != typeof(GameSaveValidator));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddGameSessions(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<GameSave>, GameSaveValidator>();
        services.AddSingleton<IGameSessionFactory, GameSessionFactory>();

        return services;
    }
}
