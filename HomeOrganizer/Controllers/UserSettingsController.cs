using AutoMapper;
using HomeOrganizer.Data;
using HomeOrganizer.DTOs;
using HomeOrganizer.Entities;
using HomeOrganizer.Extensions;
using HomeOrganizer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Controllers
{
    public class UserSettingsController : BaseApiController
    {

        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserSettingsController(IMapper mapper, DataContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserInGroupDto>>> GetAllUsersInGroup(int groupId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = await _context.Groups.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == groupId);
            if (group == null) return BadRequest();

            if (group.Users.FindIndex(x => x.UserId == user.Id) == -1)
            {
                return BadRequest();
            }
            return Ok(_mapper.Map<List<UserInGroupDto>>(group.Users));
        }
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteUserFromGroup([FromQuery]string userId, [FromQuery]string groupId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = await _context.Groups.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id.ToString() == groupId);
            if (group == null) return BadRequest();

            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
                return BadRequest();

            var userInGroup = group.Users.FirstOrDefault(x => x.UserId == userId);
            if (userInGroup != null)
            {
                group.Users.Remove(userInGroup);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<List<UserInGroupDto>>> SetPercentage([FromBody]List<UserInGroupDto> users, [FromQuery]int groupId)
        {
            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == groupId);
            if (group == null) return BadRequest();

            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            if (user == null) return BadRequest();
            if (group.Users.Exists(x => x.UserId == user.Id && x.Role == ROLE.CREATOR))
            {
                if (group.Users.Count == users.Count)
                {
                    for (int i = 0; i < group.Users.Count; i++)
                    {
                        for (int t = 0; t < users.Count; t++)
                        {
                            if (group.Users[i].UserId == users[t].UserId)
                            {
                                group.Users[i].Percent = users[t].Percent;
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                else return BadRequest();
            }
            else return BadRequest();

            return Ok(_mapper.Map<List<UserInGroup>>(group.Users));
        }
    }
}
