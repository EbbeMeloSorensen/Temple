using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Interfaces.Readers;

public interface ISitesUnlockedReader
{
    public IEnumerable<string> SitesUnlocked { get; }

    void Initialize(
        IReadOnlyCollection<string> siteIds);
}