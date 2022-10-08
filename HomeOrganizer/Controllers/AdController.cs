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
    public class AdController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public AdController(DataContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateAd([FromBody]AdDto AdDto)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(x => x.Users).Include(o => o.Ad).FirstOrDefault(p => p.Id == AdDto.GroupId);
            var UserInGroup = group.Users.FirstOrDefault(x => x.UserId == user.Id);
            Ad newAd = new();
            if (UserInGroup.Role == ROLE.CREATOR || UserInGroup.Role == ROLE.MODERATOR)
            {
                newAd.TextBody = AdDto.TextBody;
                newAd.AuthorName = user.NormalizedUserName;
                newAd.IsVoting = false;
                newAd.AuthorId = user.Id;
                if (UserInGroup.Role == ROLE.CREATOR)
                {
                    newAd.IsVoting = AdDto.IsVoting;
                    if (newAd.IsVoting == true)
                    {
                        var Voting = new Voting();
                        Voting.IsSecret = AdDto.Voting.IsSecret;
                        for (int i = 0; i < AdDto.Voting.Variants.Count; i++)
                        {
                            Variant variant = new();
                            variant.TextBody = AdDto.Voting.Variants[i].TextBody;
                            variant.Percent = 0;
                            Voting.Variants.Add(variant);
                        }
                        newAd.Voting = Voting;
                    }
                }
                group.Ad.Add(newAd);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<AdDto>>> GetAllAds([FromQuery]int GroupId)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(o => o.Users).Include(x => x.Ad).ThenInclude(c => c.Voting).ThenInclude(d => d.Variants).FirstOrDefault(p => p.Id == GroupId);
            if (group == null) return BadRequest();
            var userInGroup = group.Users.FindIndex(x => x.UserId == user.Id);
            if (userInGroup == -1) return BadRequest();
            List<AdDto> ads = _mapper.Map<List<AdDto>>(group.Ad);
            for (int i = 0; i < ads.Count; i++)
            {
                var authorId = ads[i].AuthorId;
                var authorUserInGroup = group.Users.FirstOrDefault(x => x.UserId == authorId);
                if (authorUserInGroup?.AvatarUrl == null)
                {
                    ads[i].AuthorAvatar = "https://res.cloudinary.com/dodijnztn/image/upload/v1661087474/HomeOrganizer/touch-face_l81h24.png";
                } 
                else
                {
                    ads[i].AuthorAvatar = authorUserInGroup.AvatarUrl;
                }
            }
            return Ok(ads);
        }
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<AdDto>> Vote([FromBody]MyVoteDto Vote)
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(HttpContext.User);
            var group = _context.Groups.Include(x => x.Ad).Include(l => l.Users).FirstOrDefault(p => p.Id == Vote.GroupId);
            if (group == null) return BadRequest();
            var userInGroup = group.Users.FirstOrDefault(x => x.UserId == user.Id);
            if (userInGroup == null) return BadRequest();
            var Ad = group.Ad.FirstOrDefault(x => x.Id == Vote.AdId);
            if (Ad == null) return BadRequest();
            if (Ad.Voting.IsVote.Contains(user.Id))
            {
                return BadRequest(new ProblemDetails { Title = "You are voted allready" });
            }
            var variant = Ad.Voting.Variants.FirstOrDefault(x => x.Id == Vote.VariantId);
            if (variant == null) return BadRequest();
            if (Ad.Voting.IsSecret == false)
            {
                variant.Names.Add(user.UserName);
            }
            variant.Percent += userInGroup.Percent;
            Ad.Voting.IsVote.Add(user.Id);
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<AdDto>(Ad));
        }
    }
}
