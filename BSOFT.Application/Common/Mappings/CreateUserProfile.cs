using AutoMapper;
using BSOFT.Application.Users.Commands.CreateUser;
using BSOFT.Domain.Entities;

public class CreateUserProfile : Profile
{
    public CreateUserProfile()
    {
        CreateMap<CreateUserCommand, User>();
    }
}
