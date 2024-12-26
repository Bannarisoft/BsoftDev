using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Domain.Common;

namespace BSOFT.Domain.Entities
{
    public class RoleEntitlement : BaseEntity
    {
    public int Id { get; set; }
    public int RoleId { get; set; }
    public UserRole UserRole { get; set; }
    public int ModuleId { get; set; }
    public Modules Module { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; }
    public bool CanView { get; set; }
    public bool CanAdd { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public bool CanExport { get; set; }
    }
}