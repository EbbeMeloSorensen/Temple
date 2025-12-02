using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temple.Application.Interfaces;
using Temple.Application.State.NewPrinciple;
using Temple.Persistence.EFCore.AppData;

namespace Temple.Application.Core;

public class ApplicationController
{
    private readonly GameStateMachine _gameStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    public event EventHandler<string>? ProgressChanged;

    public GameScene CurrentGameScene => _gameStateMachine.CurrentScene;

    public event Action<GameScene>? SceneChanged
    {
        add => _gameStateMachine.SceneChanged += value;
        remove => _gameStateMachine.SceneChanged -= value;
    }

    public ApplicationController(
        GameStateMachine gameStateMachine,
        IServiceScopeFactory scopeFactory,
        ILogger<ApplicationController> logger)
    {
        _gameStateMachine = gameStateMachine;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _gameStateMachine.Fire(Trigger.Initialize); // Initialize game state machine

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

            _gameStateMachine.Fire(Trigger.Initialize); // Starting → MainMenu
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
        stateMachineIO.ExportTheDamnThing(_gameStateMachine._machine);
    }

    public void StartNewGame()
    {
        _gameStateMachine.Fire(Trigger.StartNewGame);
    }

    public void ExitState()
    {
        _gameStateMachine.Fire(Trigger.ExitState);
    }

    public void GoToSmurfManagement()
    {
        _gameStateMachine.Fire(Trigger.GoToSmurfManagement);
    }

    public void GoToPeopleManagement()
    {
        _gameStateMachine.Fire(Trigger.GoToPeopleManagement);
    }

    public void GoToHome()
    {
        _gameStateMachine.Fire(Trigger.ExitState);
    }

    public void GoToDefeat()
    {
        _gameStateMachine.Fire(Trigger.GoToDefeat);
    }

    public void GoToVictory()
    {
        _gameStateMachine.Fire(Trigger.GoToVictory);
    }

    public void Shutdown()
    {
        _gameStateMachine.Fire(Trigger.ShutdownRequested);
    }

    private void Report(
        string message)
    {
        _logger.LogInformation(message);
        ProgressChanged?.Invoke(this, message);
    }
}
