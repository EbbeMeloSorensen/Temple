using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.Exploration;

public class SiteDataFactory : ISiteDataFactory
{
    public SiteData GenerateSiteData(
        string siteId)
    {
        return new SiteData
        {
            SiteComponents = SiteDataIO.ReadSiteComponentListFromFile(
                $"DD//Assets//SiteData//{siteId}.json").ToList()
        };
    }
}

