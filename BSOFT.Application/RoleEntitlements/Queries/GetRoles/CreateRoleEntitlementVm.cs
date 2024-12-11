using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.RoleEntitlements.Queries.GetRoles
{
public class CreateRoleEntitlementVm
{
    public string RoleName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedIP { get; set; }
    public List<MenuPermissionVm> MenuPermissions { get; set; } = new List<MenuPermissionVm>();
}

public class MenuPermissionVm
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