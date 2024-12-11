using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Entities
{
    [Table("MenuPermission", Schema = "AppSecurity")]
    public class MenuPermission
    {
    public int MenuPermissionId { get; set; } // Primary Key
    public string MenuName { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanExport { get; set; }
    public bool CanApprove { get; set; }

    // Optional: Link back to RoleEntitlement
    public int RoleEntitlementId { get; set; }
    public RoleEntitlement RoleEntitlement { get; set; }
    }
}