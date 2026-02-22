using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Rules;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.UnitTest;

public class QuestIOTest
{
    [Fact]
    public void WriteQuestsToFileThenReadItAndWriteItAgain()
    {
        var quests = new List<Quest>
        {
            new (
                id: "rat_infestation",
                new List<IQuestRule>
                {
                    new AdvanceOnCheatRule(),
                    new BecomeAvailableOnQuestDiscoveredRule(),
                    new AcceptQuestRule(),
                    new SatisfyOnBattleWonRule("rats_in_warehouse"),
                    new TurnInOnDialogueRule("alyth")
                }),
            new(
                id: "skeleton_trouble",
                new List<IQuestRule>
                {
                    new AdvanceOnCheatRule(),
                    new BecomeAvailableOnQuestDiscoveredRule(),
                    new AcceptQuestRule(),
                    new SatisfyOnBattleWonRule("skeletons_in_graveyard"),
                    new TurnInOnDialogueRule("captain")
                }),
            new(
                id: "bottle_for_nebbish",
                new List<IQuestRule>
                {
                    new AdvanceOnCheatRule(),
                    new BecomeAvailableOnQuestDiscoveredRule(),
                    new AcceptQuestRule(),
                    new TurnInOnDialogueRule("captain")
                }),
            new(
                id: "find_ethon",
                new List<IQuestRule>
                {
                    new AdvanceOnCheatRule(),
                    new BecomeAvailableOnQuestDiscoveredRule(),
                    new AcceptQuestRule(),
                    new SatisfyOnDialogueRule("ethon"),
                    new TurnInOnDialogueRule("alyth")
                }),
            new(
                id: "find_osalas_man",
                new List<IQuestRule>
                {
                    new AdvanceOnCheatRule(),
                    new BecomeAvailableOnQuestDiscoveredRule(),
                    new AcceptQuestRule(),
                    new SatisfyOnBattleWonRule("orb_of_the_undead"),
                    new TurnInOnDialogueRule("osala")
                }),
            new(
                id: "find_medallion_for_ipswitch",
                new List<IQuestRule>
                {
                    new AdvanceOnCheatRule(),
                    new BecomeAvailableOnQuestDiscoveredRule(),
                    new AcceptQuestRule(),
                    new TurnInOnDialogueRule("ipswitch")
                })
        };

        var fileName = @"C:\Temp\quests.json";

        // Act (Write)
        quests.WriteToFile(fileName);

        // Act (Read)
        var quests2 = QuestIO.ReadQuestListFromFile(fileName);

        // Act (Write the one that was read)
        quests2.WriteToFile(@"C:\Temp\quests_in_out.json");
    }
}