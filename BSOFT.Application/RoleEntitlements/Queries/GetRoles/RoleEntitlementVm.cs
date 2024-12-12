using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.RoleEntitlements.Queries.GetRoles
{
    public class RoleEntitlementVm : IMapFrom<RoleEntitlement>
    {
        public int RoleEntitlementId { get; set; }
        public int RoleId { get; set; }
        // public List<string> Role { get; set; }                                 
        public List<MenuPermissionVm> MenuPermissions { get; set; } = new List<MenuPermissionVm>();
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedIP { get; set; } 
    }
}