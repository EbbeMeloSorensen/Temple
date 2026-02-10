namespace Temple.Domain.Entities.DD.Exploration;

public class SiteComponent_Rotatable :SiteComponent_Placeable
{
    public double Orientation { get; set; }

    public SiteComponent_Rotatable(
        string modelId) : base(modelId)
    {
    }
}

