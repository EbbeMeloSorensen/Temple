using Microsoft.Extensions.Logging.Abstractions;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Entities.DD.Quests.Rules;

namespace Temple.Application.UnitTest;

public class GameSessionTests
{
    [Fact]
    public void StartingAnotherGame_ReplacesAllProgress_AndDisconnectsOldEvents()
    {
        var loader = new QuestLoader();
        var machine = new ApplicationStateMachine();
        using var controller = new ApplicationController(machine, null!, new GameSessionFactory(loader),
            NullLogger<ApplicationController>.Instance);
        machine.Fire(ApplicationStateShiftTrigger.Initialize);
        controller.StartNewGame();
        var first = controller.CurrentSession;
        first.EventBus.Publish(new DialogueEvent("alyth"));
        first.EventBus.Publish(new QuestAcceptedEvent("rats"));
        first.EventBus.Publish(new BattleWonEvent("warehouse"));
        first.EventBus.Publish(new DialogueEvent("alyth"));
        first.EventBus.Publish(new KnowledgeGainedEvent("secret"));
        first.EventBus.Publish(new FactEstablishedEvent("fact"));
        first.EventBus.Publish(new SiteUnlockedEvent("mine"));
        Assert.Equal(QuestState.Completed, Assert.Single(first.Quests).State);
        Assert.True(first.Queries.IsBattleWon("warehouse"));
        Assert.True(first.Queries.IsKnowledgeGained("secret"));
        Assert.True(first.Queries.IsFactEstablished("fact"));
        Assert.Contains("mine", first.SitesUnlockedReader.SitesUnlocked);

        ReturnToMainMenu(machine);
        // Consumers observing GameStartup must already see the fully initialized new session.
        GameSession? observed = null;
        controller.ApplicationStateChanged += state =>
        {
            if (state.StateMachineState == StateMachineState.GameStartup)
                observed = controller.CurrentSession;
        };
        controller.StartNewGame();
        var second = controller.CurrentSession;

        Assert.Same(second, observed);
        Assert.NotSame(first, second);
        Assert.NotSame(first.Quests.Single(), second.Quests.Single());
        Assert.Equal(2, loader.QuestLoads);
        Assert.Equal(QuestState.Hidden, second.Quests.Single().State);
        Assert.False(second.Quests.Single().AreCompletionCriteriaSatisfied);
        Assert.True(second.Queries.DoesQuestStatusEqualRequiredValue("rats", new QuestStatus
        {
            QuestState = QuestState.Hidden, AreCompletionCriteriaSatisfied = false
        }));
        Assert.False(second.Queries.IsBattleWon("warehouse"));
        Assert.False(second.Queries.IsKnowledgeGained("secret"));
        Assert.False(second.Queries.IsFactEstablished("fact"));
        Assert.Equal(new[] { "village" }, second.SitesUnlockedReader.SitesUnlocked);
        Assert.Empty(second.ApplicationData.BattlesWon);
        Assert.Empty(second.ApplicationData.KnowledgeGained);
        Assert.Empty(second.ApplicationData.FactsEstablished);
        Assert.Equal(new[] { "village" }, second.ApplicationData.SitesUnlocked);
        Assert.Equal(2, second.ApplicationData.Party.Count);
        Assert.Equal("village", second.ApplicationData.CurrentSiteId);
        Assert.Null(second.ApplicationData.ExplorationPosition);
        Assert.Throws<ObjectDisposedException>(() => first.EventBus.Publish(new BattleWonEvent("old")));

        second.EventBus.Publish(new DialogueEvent("alyth"));
        Assert.Equal(QuestState.Available, second.QuestStatusReader.GetQuestStatus("rats").QuestState);
        Assert.Equal(QuestState.Completed, first.Quests.Single().State);
    }

    [Fact]
    public void FailedNewGameCreation_PreservesPreviousSession()
    {
        var loader = new QuestLoader();
        var machine = new ApplicationStateMachine();
        using var controller = new ApplicationController(machine, null!, new GameSessionFactory(loader),
            NullLogger<ApplicationController>.Instance);
        machine.Fire(ApplicationStateShiftTrigger.Initialize);
        controller.StartNewGame();
        var first = controller.CurrentSession;
        ReturnToMainMenu(machine);
        loader.Fail = true;

        Assert.Throws<InvalidOperationException>(controller.StartNewGame);

        Assert.Same(first, controller.CurrentSession);
        Assert.Equal(StateMachineState.MainMenu, controller.CurrentApplicationState.StateMachineState);
        first.EventBus.Publish(new KnowledgeGainedEvent("still-active"));
        Assert.True(first.Queries.IsKnowledgeGained("still-active"));
    }

    private static void ReturnToMainMenu(ApplicationStateMachine machine)
    {
        machine.Fire(ApplicationStateShiftTrigger.ExitState);
        machine.Fire(ApplicationStateShiftTrigger.GoToExploration);
        machine.Fire(ApplicationStateShiftTrigger.GoToBattle);
        machine.Fire(ApplicationStateShiftTrigger.GoToVictory);
        machine.Fire(ApplicationStateShiftTrigger.ExitState);
        Assert.Equal(StateMachineState.MainMenu, machine.CurrentState.StateMachineState);
    }

    private sealed class QuestLoader : IGameIOHandler
    {
        public int QuestLoads { get; private set; }
        public bool Fail { get; set; }
        public IEnumerable<string> ReadSiteIdsFromDirectory(string directoryPath) => new[] { "village", "mine" };
        public IEnumerable<Quest> ReadQuestListFromFile(string fileName)
        {
            if (Fail) throw new InvalidOperationException("Content unavailable");
            QuestLoads++;
            return new[] { new Quest("rats", new IQuestRule[]
            {
                new BecomeAvailableOnDialogueRule("alyth"), new AcceptQuestRule(),
                new SatisfyOnBattleWonRule("warehouse"), new TurnInOnDialogueRule("alyth")
            }) };
        }
        public void WriteQuestsToFile(IEnumerable<Quest> quests, string fileName) => throw new NotSupportedException();
    }
}
