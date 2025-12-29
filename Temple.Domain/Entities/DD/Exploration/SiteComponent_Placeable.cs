using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public class SiteComponent_Placeable : SiteComponent
{
    public Vector3D Position { get; set; }

    public SiteComponent_Placeable(
        string modelId) : base(modelId)
    {
    }
}

