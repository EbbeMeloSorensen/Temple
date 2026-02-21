using Temple.Domain.Entities.DD.Exploration;

namespace Temple.Application.Interfaces;

public interface ISiteDataFactory
{
    SiteData GenerateSiteData(
        string siteId);
}