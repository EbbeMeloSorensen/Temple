using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temple.Domain.Entities.DD.Quests;
using Temple.Persistence;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Application.State.Payloads;
using Temple.Application.Saves;
using FluentValidation;
using MediatR;

namespace Temple.Application.Core;

public class ApplicationController : IDisposable
{
    private readonly ApplicationStateMachine _applicationStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    private readonly IGameSessionFactory _sessionFactory;
    private GameSession? _session;
    private readonly IGameSaveStore _saveStore;
    private readonly IValidator<GameSave> _saveValidator;
    private int _saveLoadInProgress;

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
        IGameSaveStore saveStore,
        IValidator<GameSave> saveValidator,
        ILogger<ApplicationController> logger)
    {
        _applicationStateMachine = applicationStateMachine;
        _scopeFactory = scopeFactory;
        _logger = logger;

        _sessionFactory = sessionFactory;
        _saveStore = saveStore;
        _saveValidator = saveValidator;
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
        if (Volatile.Read(ref _saveLoadInProgress) != 0)
            throw new InvalidOperationException("Please wait for the save or load to finish.");
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

    public async Task<Result<Unit>> SaveGameAsync(int slot, CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _saveLoadInProgress, 1, 0) != 0)
            return Result<Unit>.Failure("A save or load is already in progress.");
        try
        {
            if (slot is < 1 or > 3) return Result<Unit>.Failure("Choose a save slot from 1 to 3.");
            var state = CurrentApplicationState;
            if (state.StateMachineState != StateMachineState.InGameMenu ||
                state.Payload is not InGameMenuPayload menu)
                return Result<Unit>.Failure("Open the in-game menu to save a paused game.");
            var location = menu.PayloadForNextState switch
            {
                ExplorationPayload => GameSaveLocation.Exploration,
                WildernessPayload => GameSaveLocation.Wilderness,
                _ => throw new InvalidOperationException("This game state cannot be saved.")
            };
            // Capture synchronously while gameplay is stopped, before the first await.
            var snapshot = CurrentSession.Capture(location);
            _saveValidator.ValidateAndThrow(snapshot);
            await _saveStore.WriteAsync(slot, snapshot, cancellationToken);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (OperationCanceledException)
        {
            return Result<Unit>.Failure("Saving was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Saving game failed.");
            return Result<Unit>.Failure("The game could not be saved. " + ex.Message);
        }
        finally { Interlocked.Exchange(ref _saveLoadInProgress, 0); }
    }

    public async Task<Result<Unit>> LoadGameAsync(int slot, CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _saveLoadInProgress, 1, 0) != 0)
            return Result<Unit>.Failure("A save or load is already in progress.");
        GameSession? candidate = null;
        try
        {
            if (slot is < 1 or > 3) return Result<Unit>.Failure("Choose a save slot from 1 to 3.");
            var state = CurrentApplicationState;
            var previous = _session;
            if (state.StateMachineState is not (StateMachineState.MainMenu or StateMachineState.InGameMenu))
                return Result<Unit>.Failure("Load a game from the main menu or in-game menu.");
            var snapshot = await _saveStore.ReadAsync(slot, cancellationToken);
            candidate = _sessionFactory.Restore(snapshot);
            cancellationToken.ThrowIfCancellationRequested();
            if (!ReferenceEquals(state, CurrentApplicationState) || !ReferenceEquals(previous, _session))
                return Result<Unit>.Failure("The game changed while loading. Please try again.");

            _applicationStateMachine.NextPayload = snapshot.Location == GameSaveLocation.Exploration
                ? new ExplorationPayload { SiteId = candidate.ApplicationData.CurrentSiteId }
                : new WildernessPayload();
            _session = candidate;
            candidate = null;
            previous?.Dispose();
            _applicationStateMachine.Fire(ApplicationStateShiftTrigger.LoadGame);
            return Result<Unit>.Success(Unit.Value);
        }
        catch (OperationCanceledException)
        {
            return Result<Unit>.Failure("Loading was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loading game failed.");
            return Result<Unit>.Failure("The game could not be loaded. " + ex.Message);
        }
        finally
        {
            candidate?.Dispose();
            Interlocked.Exchange(ref _saveLoadInProgress, 0);
        }
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
