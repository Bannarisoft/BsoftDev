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
    // public Guid Id { get; set; }
    // public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public bool IsFirstTimeUser { get; set; } = false;
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CoId { get; set; }
    public int UnitId { get; set; }
    public int DivId { get; set; }
    public int RoleId { get; set; }
    // public List<string> Roles { get; set; }
    // public int CreatedBy { get; set; }
    // public DateTime CreatedAt { get; set; }
    // public string? CreatedByName { get; set; }
    // public string? CreatedIP { get; set; }
    }
}