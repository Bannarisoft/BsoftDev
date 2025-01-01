using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<int>
    {
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public byte IsFirstTimeUser { get; set; } = 0;
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CompanyId { get; set; }
    public int UnitId { get; set; }
    public int DivisionId { get; set; }
    public int UserRoleId { get; set; }

    }
}