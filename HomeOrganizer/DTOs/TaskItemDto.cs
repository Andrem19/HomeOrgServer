using HomeOrganizer.Entities;

namespace HomeOrganizer.DTOs
{
    public class TaskItemDto
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public TYPE Type { get; set; }
        public bool Complete { get; set; }
        public string? NameWhoCompletLast { get; set; }
        public int PayloadId { get; set; }
        public int GroupId { get; set; }
    }
}
