namespace Temple.Domain.Entities.DD.Exploration;

public abstract class SiteComponent
{
    public string ModelId { get; init; }

    protected SiteComponent(
        string modelId)
    {
        ModelId = modelId;
    }
}

