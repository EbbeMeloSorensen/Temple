using System.Collections;
using Temple.Domain.Entities.DD.Exploration;

namespace Temple.Application.Interfaces;

public interface ISiteRenderer
{
    ISiteModel Build(
        SiteData siteData);

    // Denne skal bygge et udsnit af 3D-modellen,
    // afhængigt af hvor spilleren befinder sig.
    // Som udgangspunkt får den basis-objekter, så du skal
    // lige finde ud af, hvordan du giver den, hvad den skal have for at
    // lave f.eks. npc´er. Måske skal du lave en specialicering af Circle2D
    ISiteModel Build(
        IEnumerable geometricObjects);
}