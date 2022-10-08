namespace HomeOrganizer.Entities
{
    public enum TYPE
    {
        EVERYDAY,
        ALLWAYS,
        ONCE
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TYPE Type { get; set; }
        public bool Complete { get; set; } = false;
        public string? NameWhoCompletLast { get; set; }

        public int PayloadId { get; set; }
        public Payload? Payload { get; set; }
    }
}
