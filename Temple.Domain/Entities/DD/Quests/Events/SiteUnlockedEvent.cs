namespace Temple.Domain.Entities.DD.Quests.Events;

public sealed class SiteUnlockedEvent : IGameEvent
{
    public string SiteId { get; }

    public SiteUnlockedEvent(string siteId)
    {
        SiteId = siteId;
    }
}