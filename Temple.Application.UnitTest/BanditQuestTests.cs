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
                id: "rat_infestation",
                rules: new IQuestRule[]
                {
                    new BecomeAvailableOnDialogueRule("innkeeper"),
                    new AcceptQuestRule(),
                    new CompleteOnBattleWonRule("rats_in_warehouse"),
                    new TurnInOnDialogueRule("innkeeper")
                }
            );

            var eventBus = new QuestEventBus();

            _ = new QuestRuntime(new[] { quest }, eventBus);

            // Initial sanity check
            Assert.Equal(QuestState.Hidden, quest.State);

            // --------------------
            // Act & Assert (stepwise)
            // --------------------

            // Talk to innkeeper => quest becomes available
            eventBus.Publish(new DialogueEvent("innkeeper"));
            Assert.Equal(QuestState.Available, quest.State);

            // Player accepts quest => quest becomes active
            eventBus.Publish(new QuestAcceptedEvent("rat_infestation"));
            Assert.Equal(QuestState.Active, quest.State);

            // Kill rats in warehouse => quest objectives completed
            eventBus.Publish(new BattleWonEvent("rats_in_warehouse"));
            Assert.Equal(QuestState.Active, quest.State); // still active, but ready to turn in

            // Talk to innkeeper again => quest completes
            eventBus.Publish(new DialogueEvent("innkeeper"));
            Assert.Equal(QuestState.Completed, quest.State);
        }
    }
}