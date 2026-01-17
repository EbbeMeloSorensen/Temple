using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class BecomeAvailableOnAreaEnterRule : IQuestRule
{
    private readonly string _siteId;

    public BecomeAvailableOnAreaEnterRule(
        string siteId)
    {
        _siteId = siteId;
    }

    public void Apply(
        Quest quest,
        IGameEvent e)
    {
        if (quest.State == QuestState.Hidden &&
            e is SiteEnteredEvent entered &&
            entered.SiteId == _siteId)
        {
            quest.TransitionTo(QuestState.Available);
        }
    }
}