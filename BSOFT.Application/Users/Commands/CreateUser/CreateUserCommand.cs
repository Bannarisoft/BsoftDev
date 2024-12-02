using BSOFT.Application.Users.Queries.GetUsers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<UserVm>
    {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public string UserPassword { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    }
}