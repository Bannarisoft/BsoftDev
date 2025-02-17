using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class Menu : BaseEntity
    {
    public int Id { get; set; }
    public string? MenuName { get; set; }
    public int ModuleId { get; set; }
    public int ParentId { get; set; }
    public Menu Parent { get; set; }
    public List<Menu> ChildMenus { get; set; } = new();
    public string MenuUrl { get; set; }
    public string MenuIcon { get; set; }
    public int SortOrder  { get; set; }
    public Modules? Module { get; set; }
    public IList<RoleMenu> RoleMenus { get; set; }

    }
}