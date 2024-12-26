using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Domain.Entities
{
    public class Menu
    {
    public int Id { get; set; }
    public string MenuName { get; set; }
    public int ModuleId { get; set; }
    public bool IsDeleted { get; set; }
    public Modules Module { get; set; }
    public ICollection<RoleEntitlement> RoleEntitlements { get; set; } = new List<RoleEntitlement>();
    }
}