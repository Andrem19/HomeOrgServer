namespace HomeOrganizer.Entities
{
    public class Payload
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskItem> Tasks { get; set; } = new();

        public int GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
