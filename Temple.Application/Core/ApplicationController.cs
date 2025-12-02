using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Persistence.EFCore.AppData;

namespace Temple.Application.Core;

public class ApplicationController
{
    private readonly ApplicationStateMachine _applicationStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    public event EventHandler<string>? ProgressChanged;

    public ApplicationState CurrentApplicationState => _applicationStateMachine.CurrentScene;

    public event Action<ApplicationState>? SceneChanged
    {
        add => _applicationStateMachine.SceneChanged += value;
        remove => _applicationStateMachine.SceneChanged -= value;
    }

    public ApplicationController(
        ApplicationStateMachine applicationStateMachine,
        IServiceScopeFactory scopeFactory,
        ILogger<ApplicationController> logger)
    {
        _applicationStateMachine = applicationStateMachine;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _applicationStateMachine.Fire(Trigger.Initialize); // Initialize game state machine

        Report("Initializing application...");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PRDbContextBase>();

            Report("Applying database migrations...");
            await db.Database.MigrateAsync();

            Report("Seeding database...");
            await Seeding.SeedDatabase(db);

            Report("Finalizing startup...");
            await Task.Delay(500); // Simulate additional initialization

            _applicationStateMachine.Fire(Trigger.Initialize); // Starting → MainMenu
            Report("Application is ready.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initialization failed.");
            // Optionally: add a Failed state in your state machine
        }
    }

    public void ExportStateMachineAsGraph()
    {
        using var scope = _scopeFactory.CreateScope();
        var stateMachineIO = scope.ServiceProvider.GetRequiredService<IStateMachineIO>();
        stateMachineIO.ExportTheDamnThing(_applicationStateMachine._machine);
    }

    public void StartNewGame()
    {
        _applicationStateMachine.Fire(Trigger.StartNewGame);
    }

    public void ExitState()
    {
        _applicationStateMachine.Fire(Trigger.ExitState);
    }

    public void GoToSmurfManagement()
    {
        _applicationStateMachine.Fire(Trigger.GoToSmurfManagement);
    }

    public void GoToPeopleManagement()
    {
        _applicationStateMachine.Fire(Trigger.GoToPeopleManagement);
    }

    public void GoToHome()
    {
        _applicationStateMachine.Fire(Trigger.ExitState);
    }

    public void GoToDefeat()
    {
        _applicationStateMachine.Fire(Trigger.GoToDefeat);
    }

    public void GoToVictory()
    {
        _applicationStateMachine.Fire(Trigger.GoToVictory);
    }

    public void Shutdown()
    {
        _applicationStateMachine.Fire(Trigger.ShutdownRequested);
    }

    private void Report(
        string message)
    {
        _logger.LogInformation(message);
        ProgressChanged?.Invoke(this, message);
    }
}
