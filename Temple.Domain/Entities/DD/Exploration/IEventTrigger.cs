using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public interface IEventTrigger : ISiteComponent
{
    public Point2D Point1 { get; set; }
    public Point2D Point2 { get; set; }
    public string EventID { get; set; }
}