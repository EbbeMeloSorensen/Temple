namespace Temple.ViewModel.DD.Exploration
{
    public class SiteSpecs
    {
        public string SiteId { get; set; }
        public List<ExplorationEventTrigger> ExplorationEventTriggers { get; }

        public SiteSpecs(
            string siteId,
            List<ExplorationEventTrigger> explorationEventTriggers)
        {
            SiteId = siteId;
            ExplorationEventTriggers = explorationEventTriggers;
        }
    }
}
