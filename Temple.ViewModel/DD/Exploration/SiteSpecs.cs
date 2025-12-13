using Craft.Math;

namespace Temple.ViewModel.DD.Exploration
{
    public class SiteSpecs
    {
        public List<List<Point2D>> WallPolyLines { get; }
        public List<ExplorationEventTrigger> ExplorationEventTriggers { get; }

        public SiteSpecs(
            List<List<Point2D>> wallPolyLines,
            List<ExplorationEventTrigger> explorationEventTriggers)
        {
            WallPolyLines = wallPolyLines;
            ExplorationEventTriggers = explorationEventTriggers;
        }
    }
}
