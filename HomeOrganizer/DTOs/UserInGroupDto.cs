using HomeOrganizer.Entities;

namespace HomeOrganizer.DTOs
{
    public class UserInGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }
        public string Role { get; set; }
        public double Percent { get; set; }
    }
}
