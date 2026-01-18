using Temple.Application.Core;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Domain.Entities.DD.Quests.Rules;

namespace Temple.Application.UnitTest
{
    public class BanditQuestTests
    {
        [Fact]
        public void BanditTroubleQuest_Completes_WhenPlayerFollowsIntendedFlow()
        {
            // --------------------
            // Arrange
            // --------------------

            var quest = new Quest(
                id: "bandit_trouble",
                rules: new IQuestRule[]
                {
                    new BecomeAvailableOnDialogueRule("mayor"),
                    new AcceptQuestRule(),
                    new CompleteOnEnemyDefeatedRule("bandit_leader"),
                    new TurnInOnDialogueRule("mayor")
                }
            );

            var eventBus = new QuestEventBus();

            _ = new QuestRuntime(new[] { quest }, eventBus);

            // Initial sanity check
            Assert.Equal(QuestState.Hidden, quest.State);

            // --------------------
            // Act & Assert (stepwise)
            // --------------------

            // Talk to mayor => quest becomes available
            eventBus.Publish(new DialogueEvent("mayor"));
            Assert.Equal(QuestState.Available, quest.State);

            // Player accepts quest
            eventBus.Publish(new QuestAcceptedEvent("bandit_trouble"));
            Assert.Equal(QuestState.Active, quest.State);

            // Kill bandit leader
            eventBus.Publish(new EnemyDefeatedEvent("bandit_leader"));
            Assert.Equal(QuestState.Active, quest.State); // still active, but ready to turn in

            // Talk to mayor again => quest completes
            eventBus.Publish(new DialogueEvent("mayor"));
            Assert.Equal(QuestState.Completed, quest.State);
        }
    }
}