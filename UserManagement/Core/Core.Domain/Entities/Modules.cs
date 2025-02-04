using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class Modules
    {
    public int Id { get; set; }
    public string? ModuleName { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<RoleEntitlement> RoleEntitlements { get; set; } = new List<RoleEntitlement>();
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}