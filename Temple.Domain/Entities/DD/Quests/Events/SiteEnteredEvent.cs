namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class SiteEnteredEvent : IGameEvent
{
    public string SiteId { get; }

    public SiteEnteredEvent(
        string siteId)
    {
        SiteId = siteId;
    }
}