namespace Temple.Application.Interfaces
{
    public enum QuestStatusOld
    {
        Unavailable,
        Available,
        Started,
        Completed,
        Failed
    }

    public abstract class QuestOld
    {
        public int QuestId { get; set; }

        public string SiteIdForQuestAcquisition { get; set; }

        public string SiteIdForQuestExecution { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public QuestStatusOld StatusOld { get; set; }
    }
}
