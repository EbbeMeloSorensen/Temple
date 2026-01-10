namespace Temple.Application.Interfaces
{
    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Failed
    }

    public class Quest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; set; }
    }
}
