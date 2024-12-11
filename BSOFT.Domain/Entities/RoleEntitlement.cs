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

}