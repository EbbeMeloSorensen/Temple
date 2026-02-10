namespace Temple.Application.Interfaces.Readers;

public interface ISitesUnlockedReader
{
    public IEnumerable<string> SitesUnlocked { get; }
}