using AutoMapper;
using HomeOrganizer.Data;
using HomeOrganizer.DTOs;
using HomeOrganizer.Entities;
using HomeOrganizer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeOrganizer.Controllers
{
    public class PayLoadController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public PayLoadController(DataContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("create")]
        public async Task<ActionResult<string>> CreatePayload([FromQuery]int GroupId, [FromQuery] string Name)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(a => a.Users).Include(x => x.Payloads).ThenInclude(p => p.Tasks).FirstOrDefault(x => x.Id == GroupId);
            var UserGroup = group.Users.FirstOrDefault(x => x.UserId.ToString() == user.Id);
            Payload newPayload = new();
            if (UserGroup != null)
            {
                if (UserGroup.Role == ROLE.CREATOR || UserGroup.Role == ROLE.MODERATOR)
                {
                    newPayload.Name = Name;
                    group.Payloads.Add(newPayload);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok(newPayload.Id.ToString());
        }
        [Authorize]
        [HttpGet("payloads")]
        public async Task<ActionResult<List<PayloadDto>>> GettAllPayloads([FromQuery]int GroupId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(z => z.Users).Include(x => x.Payloads).ThenInclude(w => w.Tasks).FirstOrDefault(x => x.Id == GroupId);
            var UserGroup = group?.Users.FirstOrDefault(x => x.UserId == user.Id);
            if (UserGroup == null)
                return BadRequest();
            if (group.Payloads.Count > 0)
            {
                return Ok(_mapper.Map<List<PayloadDto>>(group.Payloads));
            }
            return Ok(null);
        }
        [Authorize]
        [HttpGet("tasks")]
        public async Task<ActionResult<List<TaskItemDto>>> GettAllTasks([FromQuery]int GroupId, [FromQuery]int PayloadId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(z => z.Users).Include(x => x.Payloads).ThenInclude(c => c.Tasks).FirstOrDefault(x => x.Id == GroupId);
            var UserGroup = group?.Users.FirstOrDefault(x => x.UserId.ToString() == user.Id);
            if (UserGroup == null)
                return BadRequest();
            var payload = group?.Payloads.FirstOrDefault(x => x.Id == PayloadId);
            if (payload == null) return BadRequest();

            return Ok(_mapper.Map<List<TaskItemDto>>(payload.Tasks));
        }
        [Authorize]
        [HttpPost("checkedChange")]
        public async Task<ActionResult<List<TaskItemDto>>> CheckedChange([FromBody]CheckedChangeDto checkedChange)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(z => z.Users).Include(x => x.Payloads).ThenInclude(c => c.Tasks).FirstOrDefault(x => x.Id == checkedChange.GroupId);
            var UserGroup = group?.Users.FirstOrDefault(x => x.UserId.ToString() == user.Id);
            if (UserGroup == null)
                return BadRequest();
            var payload = group?.Payloads.FirstOrDefault(x => x.Id == checkedChange.PayloadId);
            if (payload == null) return BadRequest();
            var task = payload.Tasks.FirstOrDefault(x => x.Id == checkedChange.TaskId);

            task.Complete = checkedChange.Value;
            task.NameWhoCompletLast = user.UserName;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<List<TaskItemDto>>(payload.Tasks));
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateTask([FromBody]TaskItemDto TaskItem)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(f => f.Users).Include(x => x.Payloads).ThenInclude(x => x.Tasks).FirstOrDefault(x => x.Id == TaskItem.GroupId);
            var UserGroup = group.Users.FirstOrDefault(x => x.UserId.ToString() == user.Id);
            var payload = group.Payloads.FirstOrDefault(x => x.Id == TaskItem.PayloadId);
            TaskItem newTask = new();
            if (UserGroup != null)
            {
                if (UserGroup.Role == ROLE.CREATOR || UserGroup.Role == ROLE.MODERATOR)
                {
                    if (payload != null)
                    {
                        newTask.Title = TaskItem.Title;
                        newTask.Description = TaskItem.Description;
                        newTask.Type = TaskItem.Type;
                        newTask.Complete = false;
                        payload.Tasks.Add(newTask);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Ok();
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<PayloadDto>> ChangeTask([FromBody]TaskItemDto TaskItem)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var group = _context.Groups.Include(x => x.Payloads).ThenInclude(x => x.Tasks).FirstOrDefault(x => x.Id == TaskItem.GroupId);
            if (group == null) return BadRequest();

            var UserInGroup = group.Users.FirstOrDefault(x => x.Id.ToString() == user.Id);
            if (UserInGroup == null) return BadRequest();

            var payload = group.Payloads.FirstOrDefault(x => x.Id == TaskItem.PayloadId);
            if (payload == null) return BadRequest();

            var Task = payload.Tasks.FirstOrDefault(x => x.Id == TaskItem.Id);
            if (Task == null) return BadRequest();

            if (UserInGroup.Role == ROLE.CREATOR || UserInGroup.Role == ROLE.MODERATOR)
            {
                Task.Title = TaskItem.Title;
                Task.NameWhoCompletLast = user.UserName;
                Task.Description = TaskItem.Description;
                Task.Type = TaskItem.Type;
                Task.Complete = TaskItem.Complete;
            }
            Task.Complete = TaskItem.Complete;
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<PayloadDto>(payload));
        }
        [Authorize]
        [HttpDelete("payload")]
        public async Task<ActionResult> DeletePayload([FromQuery]int GroupId, [FromQuery] int PayloadId)
        {
            var group = _context.Groups.Include(o => o.Users).Include(g => g.Payloads).FirstOrDefault(x => x.Id == GroupId);
            if (group == null) return BadRequest();

            var payload = group.Payloads.FirstOrDefault(x => x.Id == PayloadId);
            if (payload == null) return BadRequest();

            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
                return BadRequest();

            group.Payloads.Remove(payload);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [Authorize]
        [HttpGet("reset")]
        public async Task<ActionResult> ResetPayload([FromQuery] int GroupId, [FromQuery] int PayloadId)
        {
            var group = _context.Groups.Include(o => o.Users).Include(g => g.Payloads).ThenInclude(u => u.Tasks).FirstOrDefault(x => x.Id == GroupId);
            if (group == null) return BadRequest();

            var payload = group.Payloads.FirstOrDefault(x => x.Id == PayloadId);
            if (payload == null) return BadRequest();

            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var Creators = group.Users.Where(x => x.Role == ROLE.CREATOR || x.Role == ROLE.MODERATOR).ToList();
            var CreaterWhoRequest = Creators.FirstOrDefault(x => x.UserId == user.Id);
            if (CreaterWhoRequest == null)
                return BadRequest();

            for (int i = 0; i < payload.Tasks.Count; i++)
            {
                payload.Tasks[i].Complete = false;
                payload.Tasks[i].NameWhoCompletLast = "";
            }
            await _context.SaveChangesAsync();

            return Ok();
        }
        [Authorize]
        [HttpDelete("task")]
        public async Task<ActionResult> DeleteTask([FromQuery]int TaskId, [FromQuery]int PayloadId, [FromQuery]int GroupId)
        {
            var group = _context.Groups.Include(a => a.Users).Include(s => s.Payloads).ThenInclude(q => q.Tasks).FirstOrDefault(x => x.Id == GroupId);
            if (group == null) return BadRequest();

            var payload = group.Payloads.FirstOrDefault(x => x.Id == PayloadId);
            if (payload == null) return BadRequest();

            var Task = payload.Tasks.FirstOrDefault(x => x.Id == TaskId);
            if (Task == null) return BadRequest();

            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);

            var userGroup = group.Users.FirstOrDefault(x => x.UserId == user.Id);
            if (userGroup == null) return BadRequest();

            if (userGroup.Role == ROLE.CREATOR || userGroup.Role == ROLE.MODERATOR)
            {
                payload.Tasks.Remove(Task);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
