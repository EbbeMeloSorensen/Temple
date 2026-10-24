using Temple.Domain.Entities.DD.Battle;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Rules;

namespace Temple.Domain.UnitTest;

public class ProgressRestorationTests
{
    [Fact]
    public void QuestRestorationRejectsImpossibleCompletion_WithoutChangingProgress()
    {
        var quest = new Quest("quest", Array.Empty<IQuestRule>());
        Assert.Throws<ArgumentException>(() => quest.RestoreProgress(QuestState.Completed, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => quest.RestoreProgress((QuestState)999, true));
        Assert.Equal(QuestState.Hidden, quest.State);
        quest.RestoreProgress(QuestState.Active, true);
        Assert.Equal(QuestState.Active, quest.State);
        Assert.True(quest.AreCompletionCriteriaSatisfied);
    }

    [Fact]
    public void CreatureRestorationPreservesDefeatedMembers_AndRejectsExcessHitPoints()
    {
        var creature = new Creature(new CreatureType { MaxHitPoints = 8 }, false);
        Assert.Throws<ArgumentOutOfRangeException>(() => creature.RestoreProgress(9, true));
        Assert.Equal(8, creature.HitPoints);
        creature.RestoreProgress(-2, true);
        Assert.Equal(-2, creature.HitPoints);
        Assert.True(creature.IsAutomatic);
    }
}
