using Craft.Domain;
using Craft.UIElements.Dialogs;
using Craft.ViewModels.Dialogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.People;
using Temple.Application.Smurfs;
using Temple.Application.State;
using Temple.Domain.BusinessRules.PR;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.Exploration;
using Temple.Infrastructure.IO;
using Temple.Infrastructure.Pagination;
using Temple.Infrastructure.Presentation;
using Temple.Persistence;
using Temple.Persistence.EFCore.AppData;
using Temple.ViewModel;
using Temple.ViewModel.DD.Battle.BusinessLogic;

namespace Temple.UI.WPF;

internal static class WpfServiceCollectionExtensions
{
    internal static IServiceCollection AddWpfServices(this IServiceCollection services)
    {
        var connectionString = "Data source=babuska27.db";

        services.AddAppDataPersistence<PRDbContextBase>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddApplication();
        services.AddAutoMapper(assemblies: typeof(MappingProfiles).Assembly);
        services.AddScoped<IUserAccessor, UserAccessor>();
        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IBusinessRuleCatalog, BusinessRuleCatalog>();
        services.AddScoped<IDialogService, DialogService>();
        services.AddScoped<IPagingHandler<SmurfDto>, PagingHandler<SmurfDto>>();
        services.AddScoped<IPagingHandler<PersonDto>, PagingHandler<PersonDto>>();
        services.AddScoped<IStateMachineIO, StateMachineIO>();
        services.AddGameSessions();
        services.AddGameIO(System.IO.Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData), "Temple", "Saves"));
        services.AddScoped<ISiteDataFactory, SiteDataFactory>();
        services.AddScoped<IDialogueSessionFactory, DialogueSessionFactory>();
        services.AddScoped<IBattleSceneFactory, BattleSceneFactory>();
        services.AddScoped<ISiteRenderer, WPFSiteRenderer>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MainWindow>();

        services.AddSingleton<ApplicationStateMachine>();
        services.AddSingleton<ApplicationController>();

        return services;
    }
}
