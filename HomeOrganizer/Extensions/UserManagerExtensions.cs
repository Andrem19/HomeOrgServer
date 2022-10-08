using HomeOrganizer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeOrganizer.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<User> FindByEmailFromClaimsPrinciple(this UserManager<User> input, ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);

            return await input.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
