using HomeOrganizer.Data;
using HomeOrganizer.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Helpers
{
    public static class UserInGroupCheck
    {
        public static async Task<bool> UserInGroup(DataContext _data, string groupId, User user)
        {
            var group = await _data.Groups.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == Convert.ToInt64(groupId));
            if (group != null)
            {
                int index = group.Users.FindIndex(x => x.UserId == user.Id);
                if (index != -1)
                {
                    return true;
                }
            }
            return false;
        } 
    }
}
