using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Users.Queries.GetUsers
{
    public class UserVm : IMapFrom<User>
    {
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public string UserPassword { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CoId { get; set; }
    public int? UnitId { get; set; }
    public int DivId { get; set; }
    public int RoleId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime Created_Time { get; set; }
    public string? CreatedByName { get; set; }
    public int ModifiedBy { get; set; }
    public DateTime Modified_Time { get; set; }
    public string? ModifiedByName { get; set; }
    public string Token { get; set; }
    }
}