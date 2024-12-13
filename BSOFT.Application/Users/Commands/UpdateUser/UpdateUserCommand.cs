using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<int>
    {
    // public Guid Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public bool IsFirstTimeUser { get; set; }
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CoId { get; set; }
    public int UnitId { get; set; }
    public int DivId { get; set; }
    public int RoleId { get; set; }
    public string Role { get; set; }
    // public int ModifiedBy { get; set; }
    // public DateTime ModifiedAt { get; set; }
    // public string? ModifiedByName { get; set; }
    // public string? ModifiedIP { get; set; } 

    }
}