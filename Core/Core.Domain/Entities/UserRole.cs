using Core.Domain.Common;

namespace Core.Domain.Entities
{
     
    public class UserRole  : BaseEntity
    {        
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public new byte  IsActive { get; set; } = 1;
        public ICollection<UserRoleAllocation> UserRoleAllocations { get; set; }
        public ICollection<RoleEntitlement> RoleEntitlements { get; set; } = new List<RoleEntitlement>();


    }
}
