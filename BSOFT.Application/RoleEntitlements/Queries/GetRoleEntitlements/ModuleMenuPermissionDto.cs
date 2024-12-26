using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlements.Queries.GetRoleEntitlements
{
    public class ModuleMenuPermissionDto
    {
    public int ModuleId { get; set; }
    public List<MenuPermissionDto>? Menus { get; set; }
    }
}