using Craft.Math;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Battle;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Entities.DD.Quests.Rules;
using Temple.Persistence.EFCore.AppData;

namespace Temple.Application.Core;

public class ApplicationController
{
    private readonly ApplicationStateMachine _applicationStateMachine;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ApplicationController> _logger;

    public IReadOnlyCollection<Quest> Quests { get; }

    public QuestEventBus EventBus { get; }

    public event EventHandler<string>? ProgressChanged;

    public ApplicationState CurrentApplicationState => _applicationStateMachine.CurrentState;

    public ApplicationData ApplicationData { get; private set; }

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

        // Todo: Her hardkoder vi bare et par quests. Senere læser vi dem fra fil
        var quest1 = new Quest(id: "rat_infestation", rules: new List<IQuestRule>
        {
            new AdvanceOnCheatRule(),

            // During dialogue with Alyth
            new BecomeAvailableOnQuestDiscoveredRule(),

            // Player accepts quest
            new AcceptQuestRule(),

            // Kill warehouse rats => completion criteria satisfied
            new SatisfyOnBattleWonRule("rats_in_warehouse"),

            // Talk to innkeeper again => quest completed
            new TurnInOnDialogueRule("alyth")
        });

        var quest2 = new Quest(id: "skeleton_trouble", rules: new List<IQuestRule>
        {
            new AdvanceOnCheatRule(),

            // Talk to captain => quest becomes available
            new BecomeAvailableOnQuestDiscoveredRule(),

            // Player accepts quest
            new AcceptQuestRule(),

            // Kill skeletons in graveyard => completion criteria satisfied
            new SatisfyOnBattleWonRule("skeletons_in_graveyard"),

            // Talk to captain again => quest completed
            new TurnInOnDialogueRule("captain")
        });

        var quest3 = new Quest(id: "resque_ethon", rules: new List<IQuestRule>
        {
            new AdvanceOnCheatRule(),

            // During dialogue with Alyth
            new BecomeAvailableOnQuestDiscoveredRule(),

            // Player accepts quest
            new AcceptQuestRule(),

            // Kill warehouse rats => completion criteria satisfied
            new SatisfyOnBattleWonRule("rats_in_warehouse"),

            // Talk to innkeeper again => quest completed
            new TurnInOnDialogueRule("alyth")
        });


        Quests = new List<Quest>
        {
            quest1,
            quest2
        };

        EventBus = new QuestEventBus();

        // (The QuestRuntime exists for its side effects, i.e. it is not an unused variable)
        _ = new QuestRuntime(Quests, EventBus);

        ApplicationData = new ApplicationData();

        EventBus.Subscribe<KnowledgeGainedEvent>(HandleKnowledgeGained);
        EventBus.Subscribe<BattleWonEvent>(HandleBattleWon);
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
            await Task.Delay(100); // Simulate additional initialization

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
        ApplicationData = new ApplicationData();

        // Vi starter ud med at man bare får et standard party
        GeneratePartyData();

        ApplicationData.CurrentSiteId = "Village";
        ApplicationData.ExplorationPosition = new Vector2D(14.5, -7.5);
        ApplicationData.ExplorationOrientation = 1.0 * Math.PI;

        _applicationStateMachine.NextPayload = new InterludePayload
        {
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

        ApplicationData.Party.Add(adventurer1);
        ApplicationData.Party.Add(adventurer2);
    }

    private void HandleKnowledgeGained(
        KnowledgeGainedEvent e)
    {
        ApplicationData.KnowledgeGained.Add(e.KnowledgeId);
    }

    private void HandleBattleWon(
        BattleWonEvent e)
    {
        ApplicationData.BattlesWon.Add(e.BattleId);
    }
}