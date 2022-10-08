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
    public class GroupController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;
        public GroupController(DataContext context, UserManager<User> userManager, IMapper mapper, ImageService imageService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _imageService = imageService;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<GroupDto>>> GetMyGroups()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var groups = await _context.Groups.Include(c => c.Users).Include(t => t.Ad).ToListAsync();

            var res = groups.Where(x => x.Users.Exists(p => p.UserId == user.Id));
            List<Group> groupToReturn = res.ToList();
            if (groupToReturn.Count < 1)
            {
                return Ok(null);
            }
            return Ok(_mapper.Map<List<GroupDto>>(groupToReturn));
        }
        [Authorize]
        [HttpGet("payloads")]
        public async Task<ActionResult<List<GroupDto>>> GetMyGroupsWithPayload()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var groups = await _context.Groups.Include(c => c.Users).Include(t => t.Payloads).ThenInclude(a => a.Tasks).ToListAsync();

            var res = groups.Where(x => x.Users.Exists(p => p.UserId == user.Id));
            List<Group> groupToReturn = res.ToList();
            if (groupToReturn.Count < 1)
            {
                return Ok(null);
            }
            return Ok(_mapper.Map<List<GroupDto>>(groupToReturn));
        }
        [Authorize]
        [HttpGet("roleingroup")]
        public async Task<ActionResult<string>> GetMyRole([FromQuery]string groupId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var group = await _context.Groups.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == Convert.ToInt64(groupId));

            var userInGroup = group.Users.FirstOrDefault(x => x.UserId == user.Id);
            if (userInGroup != null)
            {
                return Ok(userInGroup.Role.ToString());
            }
            return Ok();
        }
        [Authorize]
        [HttpGet("create")]
        public async Task<ActionResult<GroupDto>> CreateGroup([FromQuery]string Name)
        {
            Group group = new Group();
            group.GroupName = Name;
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            UserInGroup newuser = new();
            newuser.UserId = user.Id;
            newuser.Name = user.NormalizedUserName;
            newuser.Role = ROLE.CREATOR;
            group.Users.Add(newuser);
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<GroupDto>(group));
        }
        [Authorize]
        [HttpGet("changeName")]
        public async Task<ActionResult> ChangeName([FromQuery]string Name, [FromQuery]string groupId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == Convert.ToInt32(groupId));
            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
                return BadRequest();

            group.GroupName = Name;
            await _context.SaveChangesAsync();

            return Ok();
        }
        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteGroup([FromQuery] int GroupId)
        {
            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == GroupId);
            if (group == null) return BadRequest();

            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var Creator = group.Users.FirstOrDefault(x => x.Role == ROLE.CREATOR);
            if (Creator == null)
                return BadRequest();
            if (Creator.UserId == user.Id)
            {
                if (!string.IsNullOrEmpty(group.PicturePublicId))
                {
                    await _imageService.DeleteImageAsync(group.PicturePublicId);
                }
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [Authorize]
        [HttpPut("setRole")]
        public async Task<ActionResult> SetRole([FromBody]SetRoleDto SetRole)
        {
            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == SetRole.GroupId);
            if (group == null) return BadRequest();

            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
            {
                return BadRequest();
            }

            if (Creators.Count == 1 && SetRole.UserId == CreaterWhoRequest.UserId)
            {
                return BadRequest();
            }
            var userToChangeRole = group.Users.FirstOrDefault(x => x.UserId.ToString() == SetRole.UserId);
            if (userToChangeRole != null)
            userToChangeRole.Role = SetRole.Role;
            await _context.SaveChangesAsync();

            return Ok();
        }
        
        [Authorize]
        [HttpPut("addUser")]
        public async Task<ActionResult<Group>> AddUser([FromQuery]string InviteCode, [FromQuery]int GroupId)
        {
            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == GroupId);
            if (group == null) 
                return BadRequest();
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var userToAdd = _userManager.Users.FirstOrDefault(x=> x.InviteCode == InviteCode);
            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
                return BadRequest();

            UserInGroup newuser = new();
            newuser.UserId = userToAdd.Id;
            newuser.Name = userToAdd.UserName;
            newuser.Role = ROLE.MEMBER;
            newuser.Percent = 0;
            group.Users.Add(newuser);

            await _context.SaveChangesAsync();
            
            return group;
        }
        [Authorize]
        [HttpPost("Avatar")]
        public async Task<ActionResult> DownloadGroupPicture([FromForm]AvatarDto avatar, [FromQuery]string groupId)
        {
            if (avatar.File != null)
            {
                var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
                var group = await _context.Groups.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == Convert.ToInt64(groupId));
                if (group != null)
                {
                    var userInGroup = group.Users.FirstOrDefault(x => x.UserId == user.Id);
                    if (userInGroup?.Role == 0)
                    {
                        var imageResult = await _imageService.AddImageAsync(avatar.File);
                        if (group.PicturePublicId != null)
                        {
                            await _imageService.DeleteImageAsync(group.PicturePublicId);
                        }
                        group.PictureUrl = imageResult.SecureUrl.ToString();
                        group.PicturePublicId = imageResult.PublicId;
                        var result = await _context.SaveChangesAsync() > 0;

                        if (result) return Ok(userInGroup.AvatarUrl);

                        return BadRequest("Problem updating the group");
                    }
                }
            }
            return Ok();
        }
        [Authorize]
        [HttpGet("leaveGroup")]
        public async Task<ActionResult> LeaveGroup([FromQuery] int GroupId)
        {
            var group = _context.Groups.Include(x => x.Users).FirstOrDefault(x => x.Id == GroupId);
            if (group == null)
                return BadRequest();
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
            {
                var userToRemove = group.Users.FirstOrDefault(x => x.UserId == user.Id);
                if (userToRemove != null)
                {
                    group.Users.Remove(userToRemove);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }
    }
}
