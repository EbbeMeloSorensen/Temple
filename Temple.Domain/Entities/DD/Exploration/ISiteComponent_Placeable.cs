using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public interface ISiteComponent_Placeable : ISiteComponent
{
    Vector3D Position { get; set; }
}

