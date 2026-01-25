using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class BecomeAvailableOnQuestDiscoveredRule : IQuestRule
{
    public void Apply(
        Quest quest,
        IGameEvent e)
    {
        if (quest.State == QuestState.Hidden &&
            e is QuestDiscoveredEvent d &&
            d.QuestId == quest.Id)
        {
            quest.TransitionTo(QuestState.Available);
        }
    }
}