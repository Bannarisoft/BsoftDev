using AutoMapper;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Users.Commands.CreateUser;
using Core.Domain.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId is auto-generated
            // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id)) // Map User.Id to UserVm.UserId
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
            
    }
}
