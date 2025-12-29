using Temple.Domain.Entities.DD.Exploration;

namespace Temple.Application.Interfaces;

public interface ISiteRenderer
{
    ISiteModel Build(SiteData siteData);
}