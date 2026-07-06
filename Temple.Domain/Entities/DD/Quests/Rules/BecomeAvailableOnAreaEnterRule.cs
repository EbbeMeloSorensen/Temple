using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class BecomeAvailableOnAreaEnterRule : IQuestRule
{
    public string SiteId { get; set; }

    public BecomeAvailableOnAreaEnterRule(
        string siteId)
    {
        SiteId = siteId;
    }

    public void Apply(
        Quest quest,
        IGameEvent e)
    {
        if (quest.State == QuestState.Hidden &&
            e is SiteEnteredEvent entered &&
            entered.SiteId == SiteId)
        {
            quest.TransitionTo(QuestState.Available);
        }
    }
}