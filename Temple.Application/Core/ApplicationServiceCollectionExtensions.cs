using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Saves;

namespace Temple.Application.Core;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(People.List.Handler).Assembly));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(Smurfs.List.Handler).Assembly));

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(People.Create.Handler).Assembly));

        services.AddValidatorsFromAssembly(typeof(People.Create.CommandValidator).Assembly,
            filter: result => result.ValidatorType != typeof(GameSaveValidator));
        // The session factory and controller are singletons; this validator is stateless.
        services.AddSingleton<IValidator<GameSave>, GameSaveValidator>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddSingleton<Temple.Application.Interfaces.IGameSessionFactory, GameSessionFactory>();

        return services;
    }
}

