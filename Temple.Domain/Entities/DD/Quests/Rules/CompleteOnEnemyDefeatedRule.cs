using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class CompleteOnEnemyDefeatedRule : IQuestRule
{
    private readonly string _enemyId;

    public CompleteOnEnemyDefeatedRule(string enemyId)
    {
        _enemyId = enemyId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            e is EnemyDefeatedEvent d &&
            d.EnemyId == _enemyId)
        {
            quest.MarkObjectivesCompleted();
        }
    }
}