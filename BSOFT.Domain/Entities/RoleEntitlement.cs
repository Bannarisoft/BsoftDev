using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Entities
{
    [Table("RoleEntitlement", Schema = "AppSecurity")]
    public class RoleEntitlement
    {
    public int Id { get; set; }
    public string RoleName { get; set; }
    public List<MenuPermission> MenuPermissions { get; set; } = new List<MenuPermission>();
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }    
    
    }

    public class MenuPermission
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