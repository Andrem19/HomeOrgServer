using System.Runtime.Serialization;

namespace HomeOrganizer.Entities
{
    public enum ROLE
    {
        [EnumMember(Value = "CREATOR")]
        CREATOR,
        [EnumMember(Value = "MODERATOR")]
        MODERATOR,
        [EnumMember(Value = "MEMBER")]
        MEMBER
    }
    public class UserInGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string? AvatarUrl { get; set; }
        public string? AvatarPublicId { get; set; }
        public ROLE Role { get; set; }
        public double Percent { get; set; }

        public int GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
