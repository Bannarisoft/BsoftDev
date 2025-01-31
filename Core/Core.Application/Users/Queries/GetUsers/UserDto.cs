using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.Users.Queries.GetUsers
{
    public class UserDto : IMapFrom<User>
    {
    
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public Status IsActive { get; set; }
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CompanyId { get; set; }
    public int? UnitId { get; set; }
    public int DivisionId { get; set; }
    // public int UserRoleId { get; set; }
    public FirstTimeUserStatus IsFirstTimeUser { get; set; }
    public IsDelete IsDeleted { get; set; } 
     public List<UserCompanyDTO> UserCompanies  { get; set; }
    public List<UserRoleAllocationDTO> userRoleAllocations { get; set; }
    }
}