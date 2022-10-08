using Microsoft.AspNetCore.Identity;

namespace HomeOrganizer.Entities
{
    public class User : IdentityUser
    {
        public string InviteCode { get; set; }
    }
}
