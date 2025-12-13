using Craft.Math;

namespace Temple.ViewModel.DD.Exploration;

public abstract class ExplorationEventTrigger
{
    public Point2D Point1 { get; set; }
    public Point2D Point2 { get; set; }
    public string EventID { get; set; }
}