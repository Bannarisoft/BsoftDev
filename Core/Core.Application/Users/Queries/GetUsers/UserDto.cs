using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.Users.Queries.GetUsers
{
    public class UserDto : IMapFrom<User>
    {
    
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CompanyId { get; set; }
    public int? UnitId { get; set; }
    public int DivisionId { get; set; }
    public int UserRoleId { get; set; }
    }
}