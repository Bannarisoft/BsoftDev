using AutoMapper;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Users.Commands.UpdateUser;
using Core.Application.Users.Commands.DeleteUser;

using Core.Domain.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserCommand, User>();

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId is auto-generated
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

        CreateMap<UpdateUserCommand, User>();

        CreateMap<DeleteUserCommand, User>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId)) // Map UserRoleId to Id
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive)); // Map IsActive
            
    }
}
