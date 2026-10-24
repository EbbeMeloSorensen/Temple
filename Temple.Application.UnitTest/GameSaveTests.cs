using Craft.Math;
using Microsoft.Extensions.Logging.Abstractions;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Saves;
using Temple.Application.State;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Entities.DD.Quests.Rules;

namespace Temple.Application.UnitTest;

public class GameSaveTests
{
    [Theory]
    [InlineData(GameSaveLocation.Exploration)]
    [InlineData(GameSaveLocation.Wilderness)]
    public async Task SaveAndLoad_RestoresProgressAndQueries_WithoutReplayingRules(GameSaveLocation location)
    {
        using var game = new TestGame();
        game.OpenMenu(location);
        var first = game.Controller.CurrentSession;
        first.EventBus.Publish(new DialogueEvent("alyth"));
        first.EventBus.Publish(new QuestAcceptedEvent("rats"));
        first.EventBus.Publish(new BattleWonEvent("warehouse"));
        first.EventBus.Publish(new DialogueEvent("alyth"));
        first.EventBus.Publish(new KnowledgeGainedEvent("secret"));
        first.EventBus.Publish(new FactEstablishedEvent("fact"));
        first.EventBus.Publish(new SiteUnlockedEvent("mine"));
        first.ApplicationData.CurrentSiteId = "mine";
        first.ApplicationData.Party[0].HitPoints = -2;
        first.ApplicationData.Party[1].IsAutomatic = true;
        if (location == GameSaveLocation.Exploration)
        {
            first.ApplicationData.ExplorationPosition = new Vector2D(2.5, 7.25);
            first.ApplicationData.ExplorationOrientation = 37;
        }
        Assert.True((await game.Controller.SaveGameAsync(2)).IsSuccess);
        // A snapshot must not retain any references to mutable session progress.
        first.ApplicationData.Party[0].HitPoints = 8;
        first.EventBus.Publish(new KnowledgeGainedEvent("later"));
        var callsBeforeLoad = game.Loader.RuleCalls;

        var result = await game.Controller.LoadGameAsync(2);

        Assert.True(result.IsSuccess, result.Error);
        var restored = game.Controller.CurrentSession;
        Assert.NotSame(first, restored);
        Assert.Equal(callsBeforeLoad, game.Loader.RuleCalls);
        Assert.True(restored.Queries.IsKnowledgeGained("secret"));
        Assert.False(restored.Queries.IsKnowledgeGained("later"));
        Assert.True(restored.Queries.IsFactEstablished("fact"));
        Assert.True(restored.Queries.IsBattleWon("warehouse"));
        Assert.Contains("mine", restored.SitesUnlockedReader.SitesUnlocked);
        Assert.Contains("mine", restored.ApplicationData.SitesUnlocked);
        Assert.Contains("warehouse", restored.ApplicationData.BattlesWon);
        Assert.Contains("secret", restored.ApplicationData.KnowledgeGained);
        Assert.Contains("fact", restored.ApplicationData.FactsEstablished);
        Assert.True(restored.Queries.DoesQuestStatusEqualRequiredValue("rats", new QuestStatus
        {
            QuestState = QuestState.Completed, AreCompletionCriteriaSatisfied = true
        }));
        Assert.Equal(QuestState.Completed, restored.Quests.Single().State);
        Assert.Equal(-2, restored.ApplicationData.Party[0].HitPoints);
        Assert.True(restored.ApplicationData.Party[1].IsAutomatic);
        Assert.Equal(first.ApplicationData.Party[0].CreatureType.ID, restored.ApplicationData.Party[0].CreatureType.ID);
        Assert.Empty(restored.ApplicationData.Party[0].Attacks);
        Assert.Equal("mine", restored.ApplicationData.CurrentSiteId);
        if (location == GameSaveLocation.Exploration)
        {
            Assert.Equal(2.5, restored.ApplicationData.ExplorationPosition!.X);
            Assert.Equal(7.25, restored.ApplicationData.ExplorationPosition.Y);
            Assert.Equal(37, restored.ApplicationData.ExplorationOrientation);
            Assert.Equal("mine", Assert.IsType<ExplorationPayload>(game.Controller.CurrentApplicationState.Payload).SiteId);
        }
        else
        {
            Assert.Null(restored.ApplicationData.ExplorationPosition);
            Assert.IsType<WildernessPayload>(game.Controller.CurrentApplicationState.Payload);
        }
        Assert.Throws<ObjectDisposedException>(() => first.EventBus.Publish(new BattleWonEvent("stale")));

        // Restored readers remain subscribed to future events.
        restored.EventBus.Publish(new FactEstablishedEvent("after-load"));
        Assert.True(restored.Queries.IsFactEstablished("after-load"));
    }

    [Theory]
    [InlineData("format")]
    [InlineData("content")]
    [InlineData("site")]
    [InlineData("unlocked-site")]
    [InlineData("quest")]
    [InlineData("duplicate-quest")]
    [InlineData("quest-state")]
    [InlineData("quest-invariant")]
    [InlineData("creature")]
    [InlineData("hit-points")]
    [InlineData("missing-progress")]
    [InlineData("partial-position")]
    [InlineData("nonfinite-position")]
    [InlineData("resume-state")]
    public async Task InvalidSave_DoesNotReplaceCurrentSession(string problem)
    {
        using var game = new TestGame();
        game.OpenMenu(GameSaveLocation.Exploration);
        Assert.True((await game.Controller.SaveGameAsync(1)).IsSuccess);
        var first = game.Controller.CurrentSession;
        var state = game.Controller.CurrentApplicationState;
        var save = game.Store.Save!;
        switch (problem)
        {
            case "format": save.FormatVersion++; break;
            case "content": save.ContentVersion = "future"; break;
            case "site": save.CurrentSiteId = "removed"; break;
            case "unlocked-site": save.SitesUnlocked.Add("removed"); break;
            case "quest": save.Quests.Clear(); break;
            case "duplicate-quest": save.Quests.Add(save.Quests[0]); break;
            case "quest-state": save.Quests[0].State = (QuestState)999; break;
            case "quest-invariant": save.Quests[0].State = QuestState.Completed; break;
            case "creature": save.Party[0].CreatureTypeId = Guid.NewGuid(); break;
            case "hit-points": save.Party[0].HitPoints = int.MaxValue; break;
            case "missing-progress": save.KnowledgeGained = null!; break;
            case "partial-position": save.PositionX = 1; break;
            case "nonfinite-position": save.PositionX = double.NaN; save.PositionY = 1; save.Orientation = 1; break;
            case "resume-state": save.Location = (GameSaveLocation)999; break;
        }

        var result = await game.Controller.LoadGameAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Same(first, game.Controller.CurrentSession);
        Assert.Same(state, game.Controller.CurrentApplicationState);
        first.EventBus.Publish(new FactEstablishedEvent("still-alive"));
        Assert.True(first.Queries.IsFactEstablished("still-alive"));
    }

    [Fact]
    public async Task LoadFromMainMenu_AndThenStartNewGame_UsesFreshDefaults()
    {
        using var game = new TestGame();
        game.OpenMenu(GameSaveLocation.Wilderness);
        game.Controller.EventBus.Publish(new KnowledgeGainedEvent("saved"));
        Assert.True((await game.Controller.SaveGameAsync(1)).IsSuccess);
        game.Controller.GoToNextApplicationState(new WildernessPayload());
        game.Controller.GoToNextApplicationState(new BattlePayload());
        game.Controller.GoToVictory();
        game.Controller.ExitState();
        Assert.True((await game.Controller.LoadGameAsync(1)).IsSuccess);
        Assert.True(game.Controller.CurrentSession.Queries.IsKnowledgeGained("saved"));
        game.Controller.GoToNextApplicationState(new WildernessPayload());
        game.Controller.GoToNextApplicationState(new BattlePayload());
        game.Controller.GoToVictory();
        game.Controller.ExitState();
        game.Controller.StartNewGame();
        Assert.False(game.Controller.CurrentSession.Queries.IsKnowledgeGained("saved"));
    }

    [Fact]
    public async Task ActiveGameplayCannotBeSaved_AndConcurrentLoadsAreRejected()
    {
        using var game = new TestGame();
        game.Controller.StartNewGame();
        Assert.False((await game.Controller.SaveGameAsync(1)).IsSuccess);
        game.Controller.ExitState();
        game.Controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "village" });
        Assert.False((await game.Controller.SaveGameAsync(1)).IsSuccess);
        Assert.False((await game.Controller.LoadGameAsync(1)).IsSuccess);
        game.Controller.GoToNextApplicationState(new InGameMenuPayload { PayloadForNextState = new ExplorationPayload() });
        Assert.True((await game.Controller.SaveGameAsync(1)).IsSuccess);
        game.Store.ReadGate = new TaskCompletionSource<GameSave>();
        var first = game.Controller.LoadGameAsync(1);
        Assert.False((await game.Controller.LoadGameAsync(1)).IsSuccess);
        Assert.False((await game.Controller.SaveGameAsync(1)).IsSuccess);
        game.Store.ReadGate.SetResult(game.Store.Save!);
        Assert.True((await first).IsSuccess);
    }

    [Fact]
    public async Task MissingOrCanceledLoad_LeavesSessionUntouched()
    {
        using var game = new TestGame();
        game.OpenMenu(GameSaveLocation.Wilderness);
        var first = game.Controller.CurrentSession;
        Assert.False((await game.Controller.LoadGameAsync(3)).IsSuccess);
        using var canceled = new CancellationTokenSource();
        canceled.Cancel();
        Assert.False((await game.Controller.LoadGameAsync(1, canceled.Token)).IsSuccess);
        Assert.Same(first, game.Controller.CurrentSession);
    }

    [Fact]
    public async Task NavigatingWhileLoadIsPending_PreventsCommittingAnObsoleteLoad()
    {
        using var game = new TestGame();
        game.OpenMenu(GameSaveLocation.Wilderness);
        Assert.True((await game.Controller.SaveGameAsync(1)).IsSuccess);
        var previous = game.Controller.CurrentSession;
        game.Store.ReadGate = new TaskCompletionSource<GameSave>();
        var load = game.Controller.LoadGameAsync(1);
        game.Controller.GoToNextApplicationState(new WildernessPayload());
        game.Store.ReadGate.SetResult(game.Store.Save!);

        Assert.False((await load).IsSuccess);
        Assert.Same(previous, game.Controller.CurrentSession);
        Assert.Equal(StateMachineState.Wilderness, game.Controller.CurrentApplicationState.StateMachineState);
        previous.EventBus.Publish(new FactEstablishedEvent("still-active"));
        Assert.True(previous.Queries.IsFactEstablished("still-active"));
    }

    private sealed class TestGame : IDisposable
    {
        public RecordingLoader Loader { get; } = new();
        public MemoryStore Store { get; } = new();
        public ApplicationController Controller { get; }
        public TestGame()
        {
            var machine = new ApplicationStateMachine();
            var validator = new GameSaveValidator();
            Controller = new ApplicationController(machine, null!, new GameSessionFactory(Loader, validator),
                Store, validator, NullLogger<ApplicationController>.Instance);
            machine.Fire(ApplicationStateShiftTrigger.Initialize);
        }
        public void OpenMenu(GameSaveLocation location)
        {
            Controller.StartNewGame();
            Controller.ExitState();
            Controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "village" });
            if (location == GameSaveLocation.Wilderness) Controller.GoToWilderness();
            Controller.GoToNextApplicationState(new InGameMenuPayload
            {
                PayloadForNextState = location == GameSaveLocation.Wilderness
                    ? new WildernessPayload() : new ExplorationPayload { SiteId = "village" }
            });
        }
        public void Dispose() => Controller.Dispose();
    }

    private sealed class MemoryStore : IGameSaveStore
    {
        public GameSave? Save { get; private set; }
        public TaskCompletionSource<GameSave>? ReadGate { get; set; }
        public Task WriteAsync(int slot, GameSave save, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Save = save;
            return Task.CompletedTask;
        }
        public Task<GameSave> ReadAsync(int slot, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ReadGate?.Task ?? Task.FromResult(Save ?? throw new FileNotFoundException());
        }
    }

    private sealed class RecordingLoader : IGameIOHandler, IQuestRule
    {
        public int RuleCalls { get; private set; }
        public void Apply(Quest quest, IGameEvent gameEvent) => RuleCalls++;
        public IEnumerable<string> ReadSiteIdsFromDirectory(string path) => new[] { "village", "mine" };
        public IEnumerable<Quest> ReadQuestListFromFile(string path) => new[]
        {
            new Quest("rats", new IQuestRule[]
            {
                this, new BecomeAvailableOnDialogueRule("alyth"), new AcceptQuestRule(),
                new SatisfyOnBattleWonRule("warehouse"), new TurnInOnDialogueRule("alyth")
            })
        };
        public void WriteQuestsToFile(IEnumerable<Quest> quests, string path) => throw new NotSupportedException();
    }
}
