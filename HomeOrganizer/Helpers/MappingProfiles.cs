using HomeOrganizer.DTOs;
using HomeOrganizer.Entities;
using AutoMapper;

namespace HomeOrganizer.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Group, GroupDto>();
            CreateMap<UserInGroup, UserInGroupDto>()
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));
            CreateMap<Payload, PayloadDto>();
            CreateMap<TaskItem, TaskItemDto>();
            CreateMap<Ad, AdDto>();
            CreateMap<Voting, VotingDto>();
            CreateMap<Variant, VariantDto>();
        }
    }
}