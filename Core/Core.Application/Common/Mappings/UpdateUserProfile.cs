using AutoMapper;
using Core.Application.Users.Commands.UpdateUser;
using Core.Domain.Entities;

public class UpdateUserProfile : Profile
{
    public UpdateUserProfile()
    {
        CreateMap<UpdateUserCommand, User>();
    }
}
