using Craft.Math;

namespace Temple.ViewModel.DD.Exploration
{
    public class SiteSpecs
    {
        public string SiteId { get; set; }
        public List<List<Point2D>> WallPolyLines { get; }
        public List<ExplorationEventTrigger> ExplorationEventTriggers { get; }

        public SiteSpecs(
            string siteId,
            List<List<Point2D>> wallPolyLines,
            List<ExplorationEventTrigger> explorationEventTriggers)
        {
            SiteId = siteId;
            WallPolyLines = wallPolyLines;
            ExplorationEventTriggers = explorationEventTriggers;
        }
    }
}
