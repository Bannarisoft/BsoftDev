using AutoMapper;
using BSOFT.Application.Users.Commands.UpdateUser;
using BSOFT.Domain.Entities;

public class UpdateUserProfile : Profile
{
    public UpdateUserProfile()
    {
        CreateMap<UpdateUserCommand, User>();
    }
}
