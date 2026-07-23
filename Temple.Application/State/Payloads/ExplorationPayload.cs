using Craft.Math;

namespace Temple.Application.State.Payloads;

public class ExplorationPayload : ApplicationStatePayload
{
    public string SiteId { get; set; }

    public Point2D? StartPosition { get; set; }
    public double? StartOrientation { get; set; }
}