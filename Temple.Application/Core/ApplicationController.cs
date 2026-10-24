using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temple.Domain.Entities.DD.Quests;
using Temple.Persistence;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Application.State.Payloads;

namespace Temple.Application.Core;

public class ApplicationController : IDisposable
{
    private readonly ApplicationStateMachine _applicationStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    private readonly IGameSessionFactory _sessionFactory;
    private GameSession? _session;

    public GameSession CurrentSession => _session
        ?? throw new InvalidOperationException("No game has been started.");

    public IReadOnlyCollection<string> SiteIds => CurrentSession.SiteIds;
    public IReadOnlyCollection<Quest> Quests => CurrentSession.Quests;
    public QuestEventBus EventBus => CurrentSession.EventBus;

    public event EventHandler<string>? ProgressChanged;

    public ApplicationState CurrentApplicationState => _applicationStateMachine.CurrentState;

    public ApplicationData ApplicationData => CurrentSession.ApplicationData;

    public event Action<ApplicationState>? ApplicationStateChanged
    {
        add => _applicationStateMachine.StateChanged += value;
        remove => _applicationStateMachine.StateChanged -= value;
    }

    public ApplicationController(
        ApplicationStateMachine applicationStateMachine,
        IServiceScopeFactory scopeFactory,
        IGameSessionFactory sessionFactory,
        ILogger<ApplicationController> logger)
    {
        _applicationStateMachine = applicationStateMachine;
        _scopeFactory = scopeFactory;
        _logger = logger;

        _sessionFactory = sessionFactory;
    }

    public async Task InitializeAsync()
    {
        _applicationStateMachine.Fire(ApplicationStateShiftTrigger.Initialize); // Initialize game state machine

        Report("Initializing application...");

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IAppDataInitializer>();

            Report("Applying database migrations...");
            await initializer.MigrateAsync();

            Report("Seeding database...");
            await initializer.SeedAsync();

            //Report("Finalizing startup...");
            ///await Task.Delay(100); // Simulate additional initialization

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
        if (_applicationStateMachine.CurrentState.StateMachineState != StateMachineState.MainMenu)
        {
            throw new InvalidOperationException("A new game can only be started from the main menu.");
        }

        // Only replace the current session after its replacement is ready.
        var nextSession = _sessionFactory.CreateNew();
        _session?.Dispose();
        _session = nextSession;

        _applicationStateMachine.NextPayload = new InterludePayload
        {
            // Magic number
            Text = "Så går eventyret i gang",
            PayloadForNextState = new ExplorationPayload
            {
                SiteId = ApplicationData.CurrentSiteId
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
            case InGameMenuPayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToInGameMenu);
                break;
            case ExplorationPayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToExploration);
                break;
            case DialoguePayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToDialogue);
                break;
            case BattlePayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToBattle);
                break;
            case WildernessPayload:
                _applicationStateMachine.Fire(ApplicationStateShiftTrigger.GoToWilderness);
                break;
            default:
                throw new InvalidOperationException("Unknown payload type");
        }
    }

    public void GoToWilderness()
    {
        // Stop storing the exploration position so that the start position from
        // a site will be used if reentering that site
        ApplicationData.ExplorationPosition = null;
        ApplicationData.ExplorationOrientation = null;

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

    public void Dispose() => _session?.Dispose();

    private void Report(
        string message)
    {
        _logger.LogInformation(message);
        ProgressChanged?.Invoke(this, message);
    }

}
