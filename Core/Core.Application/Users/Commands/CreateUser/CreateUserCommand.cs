using Core.Application.Common.HttpResponse;
using Core.Application.Users.Queries.GetUsers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<ApiResponseDTO<UserDto>>
    {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public bool IsActive { get; set; }
    public bool IsFirstTimeUser { get; set; } = false;
    public string Password { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CompanyId { get; set; }
    public int UnitId { get; set; }
    public int DivisionId { get; set; }
    public int UserRoleId { get; set; }
    }
}