using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Domain.Entities
{
    public class Modules
    {
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    
    }
}