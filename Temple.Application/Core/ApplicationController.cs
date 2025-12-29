using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Craft.Math;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD;
using Temple.Persistence.EFCore.AppData;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.Application.Core;

public class ApplicationController
{
    private readonly ApplicationStateMachine _applicationStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    public event EventHandler<string>? ProgressChanged;

    public ApplicationState CurrentApplicationState => _applicationStateMachine.CurrentState;

    public ApplicationData Data { get; private set; }

    public event Action<ApplicationState>? ApplicationStateChanged
    {
        add => _applicationStateMachine.StateChanged += value;
        remove => _applicationStateMachine.StateChanged -= value;
    }

    public ApplicationController(
        ApplicationStateMachine applicationStateMachine,
        IServiceScopeFactory scopeFactory,
        ILogger<ApplicationController> logger)
    {
        _applicationStateMachine = applicationStateMachine;
        _scopeFactory = scopeFactory;
        _logger = logger;

        Data = new ApplicationData();
    }

    public async Task InitializeAsync()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.Initialize); // Initialize game state machine

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

            _applicationStateMachine.Fire(ApplicationStateShiftTrigger.Initialize); // Starting → MainMenu
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
        Data = new ApplicationData();

        // Vi starter ud med at man bare får et standard party
        GeneratePartyData();

        Data.CurrentSite = "Mine";
        Data.ExplorationPosition = new Vector2D(0.5, -0.5);
        Data.ExplorationOrientation = 0.5 * Math.PI;

        _applicationStateMachine.NextPayload = new InterludePayload
        {
            Text = "Så går eventyret i gang",
            PayloadForNextState = new ExplorationPayload
            {
                Site = Data.CurrentSite
            }
        };

        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.StartNewGame);
    }

    public void ExitState()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.ExitState);
    }

    public void GoToSmurfManagement()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToSmurfManagement);
    }

    public void GoToPeopleManagement()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToPeopleManagement);
    }

    public void GoToHome()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.ExitState);
    }

    public void GoToNextApplicationState(
        ApplicationStatePayload payload)
    {
        _applicationStateMachine.NextPayload = payload;

        switch (payload)
        {
            case ExplorationPayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToExploration);
                break;
            case BattlePayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToBattle);
                break;
            default:
                throw new InvalidOperationException("Unknown payload type");
        }
    }

    public void GoToWilderness()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToWilderness);
    }

    public void GoToDefeat()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToDefeat);
    }

    public void GoToVictory()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToVictory);
    }

    public void Shutdown()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.ShutdownRequested);
    }

    private void Report(
        string message)
    {
        _logger.LogInformation(message);
        ProgressChanged?.Invoke(this, message);
    }

    private void GeneratePartyData()
    {
        // Attacks
        var longSword = new MeleeAttack(name: "Longsword", maxDamage: 10);
        var bowAndArrow = new RangedAttack(name: "Bow & Arrow", maxDamage: 4, range: 5);

        // Creature types
        var knight = new CreatureType("Knight",
            maxHitPoints: 8, //20,
            armorClass: 3,
            thaco: 5,
            initiativeModifier: 0,
            movement: 4,
            attacks: new List<Attack>
            {
                longSword,
                longSword,
                longSword
            });

        var archer = new CreatureType(
            name: "Archer",
            maxHitPoints: 12,
            armorClass: 6,
            thaco: 14,
            initiativeModifier: 10,
            movement: 6,
            attacks: new List<Attack>
            {
                bowAndArrow,
                bowAndArrow
            });

        var adventurer1 = new Creature(knight, false);
        var adventurer2 = new Creature(archer, false);

        Data.Party.Add(adventurer1);
        Data.Party.Add(adventurer2);
    }
}
