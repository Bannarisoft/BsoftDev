using AutoMapper;
using BSOFT.Application.Users.Queries.GetUsers;
using BSOFT.Application.Users.Commands.CreateUser;
using BSOFT.Domain.Entities;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserVm>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId is auto-generated
            // .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id)) // Map User.Id to UserVm.UserId
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.ModifiedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
