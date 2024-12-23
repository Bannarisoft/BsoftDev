using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommand : IRequest<int>
    {
    public string RoleName { get; set; }
    public List<MenuPermissionDto> MenuPermissions { get; set; }
    }

    public class MenuPermissionDto
    {
    public string MenuName { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanExport { get; set; }
    public bool RequiresApproval { get; set; }
    }
}