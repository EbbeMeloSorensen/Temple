namespace Temple.Application.Interfaces
{
    public enum QuestStatus
    {
        Unavailable,
        Available,
        Started,
        Completed,
        Failed
    }

    public abstract class Quest
    {
        public int QuestId { get; set; }

        public string SiteIdForQuestAcquisition { get; set; }
        public string SiteIdForQuestExecution { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public QuestStatus Status { get; set; }
    }
}
