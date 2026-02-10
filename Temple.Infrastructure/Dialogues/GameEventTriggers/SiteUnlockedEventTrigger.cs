namespace Temple.Infrastructure.Dialogues.GameEventTriggers;

public class SiteUnlockedEventTrigger : IGameEventTrigger
{
    public string SiteId { get; set; }

    public SiteUnlockedEventTrigger(
        string siteId)
    {
        SiteId = siteId;
    }

    public override string ToString()
    {
        return $"Site unlocked: {SiteId}";
    }
}