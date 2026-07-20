using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;

namespace Temple.ViewModel.DD.ReadModels;

public class SitesUnlockedReadModel : ISitesUnlockedReader
{
    private readonly HashSet<string> _sitesUnlocked = new HashSet<string>();

    public IEnumerable<string> SitesUnlocked => _sitesUnlocked;

    public SitesUnlockedReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<SiteUnlockedEvent>(HandleSiteUnlocked);
    }

    public void Initialize(
        IReadOnlyCollection<string> siteIds)
    {
        foreach (var siteId in siteIds)
        {
            if (siteId == "graveyard")
            {
                continue;
            }

            _sitesUnlocked.Add(siteId);
        }
    }

    private void HandleSiteUnlocked(
        SiteUnlockedEvent e)
    {
        _sitesUnlocked.Add(e.SiteId);
    }
}