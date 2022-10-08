namespace HomeOrganizer.DTOs
{
    public class PayloadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskItemDto>? Tasks { get; set; } = new();
    }
}
