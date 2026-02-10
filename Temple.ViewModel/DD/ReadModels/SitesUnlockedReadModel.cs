using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests.Events;

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

    private void HandleSiteUnlocked(
        SiteUnlockedEvent e)
    {
        _sitesUnlocked.Add(e.SiteId);
    }
}