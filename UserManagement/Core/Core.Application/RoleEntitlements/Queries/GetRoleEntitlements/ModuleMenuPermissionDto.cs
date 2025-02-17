using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.RoleEntitlements.Queries.GetRoleEntitlements
{
    public class ModuleMenuPermissionDto
    {
     public int RoleId { get; set; }
     public int ModuleId { get; set; }
     public IList<MenuPermissionDto> MenuPermissions { get; set; }
    }
}