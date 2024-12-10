using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.RoleEntitlement.Queries.GetUsers
{
public class CreateRoleEntitlementDto
{
    public string RoleName { get; set; }
    public List<MenuPermissionDto> MenuPermissions { get; set; } = new List<MenuPermissionDto>();
}

public class MenuPermissionDto
{
    public string MenuName { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanExport { get; set; }
    public bool CanApprove { get; set; }
}

}