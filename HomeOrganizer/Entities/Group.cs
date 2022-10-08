namespace HomeOrganizer.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string? PictureUrl { get; set; }
        public string? PicturePublicId { get; set; }
        public List<UserInGroup> Users { get; set; } = new();
        public List<Payload> Payloads { get; set; } = new();
        public List<Ad> Ad { get; set; } = new();
    }
}
