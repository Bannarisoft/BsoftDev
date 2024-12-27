using AutoMapper;
using Core.Application.Users.Commands.CreateUser;
using Core.Domain.Entities;

public class CreateUserProfile : Profile
{
    public CreateUserProfile()
    {
        CreateMap<CreateUserCommand, User>();
    }
}
