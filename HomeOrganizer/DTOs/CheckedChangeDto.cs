namespace HomeOrganizer.DTOs
{
    public class CheckedChangeDto
    {
        public int GroupId { get; set; }
        public int PayloadId { get; set; }
        public int TaskId { get; set; }
        public bool Value { get; set; }
    }
}
