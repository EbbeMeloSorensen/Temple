using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddValidatorsFromAssembly(typeof(People.Create.CommandValidator).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

