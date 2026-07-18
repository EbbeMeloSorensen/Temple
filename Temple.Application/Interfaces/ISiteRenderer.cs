using System.Collections;
using Temple.Domain.Entities.DD.Exploration;

namespace Temple.Application.Interfaces;

public interface ISiteRenderer
{
    // Denne bygger HELE modellen (deprecated)
    ISiteModel Build(
        SiteData siteData);

    // Denne bygger et UDSNIT af den statiske del af 3D-modellen,
    // afhængigt af hvor spilleren befinder sig.
    ISiteModel BuildStaticPart(
        IEnumerable geometricObjects);

    // Denne bygger den DYNAMISKE del af 3D-modellen
    ISiteModel BuildDynamicPart(
        IEnumerable geometricObjects);

}