using HomeOrganizer.Entities;

namespace HomeOrganizer.DTOs
{
    public class SetRoleDto
    {
        public int GroupId { get; set; }
        public string UserId { get; set; }
        public ROLE Role { get; set; }
    }
}
