using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class CompleteOnBattleWonRule : IQuestRule
{
    private readonly string _battleId;

    public CompleteOnBattleWonRule(string battleId)
    {
        _battleId = battleId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            e is BattleWonEvent @event &&
            @event.BattleId == _battleId)
        {
            quest.MarkObjectivesCompleted();
        }
    }
}