using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class SatisfyOnBattleWonRule : IQuestRule
{
    public string BattleId { get; set; }

    public SatisfyOnBattleWonRule(string battleId)
    {
        BattleId = battleId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            e is BattleWonEvent @event &&
            @event.BattleId == BattleId)
        {
            quest.MarkObjectivesCompleted();
        }
    }
}