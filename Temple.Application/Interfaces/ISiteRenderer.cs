using System.Collections;

namespace Temple.Application.Interfaces;

public interface ISiteRenderer
{
    // Denne bygger et UDSNIT af den statiske del af 3D-modellen,
    // afhængigt af hvor spilleren befinder sig.
    ISiteModel Build(
        IEnumerable geometricObjects);
}