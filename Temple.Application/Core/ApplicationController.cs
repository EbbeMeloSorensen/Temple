using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temple.Application.Interfaces;
using Temple.Application.State.NewPrinciple;
using Temple.Application.State.OldPrinciple;
using Temple.Persistence.EFCore.AppData;

namespace Temple.Application.Core;

public class ApplicationController
{
    private readonly ApplicationStateMachine _stateMachine;
    private readonly GameStateMachine _gameStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    public event EventHandler<string>? ProgressChanged;

    public ApplicationState CurrentState => _stateMachine.CurrentState;

    public event EventHandler<ApplicationStateChangedEventArgs> StateChanged
    {
        add => _stateMachine.StateChanged += value;
        remove => _stateMachine.StateChanged -= value;
    }

    public ApplicationController(
        ApplicationStateMachine stateMachine,
        GameStateMachine gameStateMachine,
        IServiceScopeFactory scopeFactory,
        ILogger<ApplicationController> logger)
    {
        _stateMachine = stateMachine;
        _gameStateMachine = gameStateMachine;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _stateMachine.Fire(ApplicationTrigger.Initialize); // Starting → (still Starting)
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

            _stateMachine.Fire(ApplicationTrigger.Initialize); // Starting → MainMenu
            Report("Application is ready.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initialization failed.");
            // Optionally: add a Failed state in your state machine
        }
    }

    //public void BeginWork()
    //{
    //    _stateMachine.Fire(ApplicationTrigger.WorkRequested);
    //    Task.Run(async () =>
    //    {
    //        try
    //        {
    //            await Task.Delay(2000); // simulate work
    //            _stateMachine.Fire(ApplicationTrigger.WorkCompleted);
    //        }
    //        catch
    //        {
    //            _stateMachine.Fire(ApplicationTrigger.ErrorOccurred);
    //        }
    //    });
    //}

    public void ExportStateMachineAsGraph()
    {
        using var scope = _scopeFactory.CreateScope();
        var stateMachineIO = scope.ServiceProvider.GetRequiredService<IStateMachineIO>();
        stateMachineIO.ExportTheDamnThing(_stateMachine._machine);
    }

    public void StartNewGame()
    {
        _stateMachine.Fire(ApplicationTrigger.StartNewGame);
    }

    public void ExitState()
    {
        _stateMachine.Fire(ApplicationTrigger.ExitState);
    }

    public void GoToSmurfManagement()
    {
        _stateMachine.Fire(ApplicationTrigger.GoToSmurfManagement);
    }

    public void GoToPeopleManagement()
    {
        _stateMachine.Fire(ApplicationTrigger.GoToPeopleManagement);
    }

    public void GoToHome()
    {
        _stateMachine.Fire(ApplicationTrigger.GoToHome);
    }

    public void GoToDefeat()
    {
        _stateMachine.Fire(ApplicationTrigger.GoToDefeat);
    }

    public void GoToVictory()
    {
        _stateMachine.Fire(ApplicationTrigger.GoToVictory);
    }

    public void Shutdown()
    {
        _stateMachine.Fire(ApplicationTrigger.ShutdownRequested);
    }

    private void Report(
        string message)
    {
        _logger.LogInformation(message);
        ProgressChanged?.Invoke(this, message);
    }
}
