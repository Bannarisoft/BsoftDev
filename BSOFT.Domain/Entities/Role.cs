using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSOFT.Domain.Common;

namespace BSOFT.Domain.Entities
{
    [Table("Role", Schema = "AppSecurity")]
    public class Role  : BaseEntity
    {
        
 
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
       
        public List<User> Users { get; set; } = new List<User>();
        public List<RoleEntitlement> RoleEntitlements { get; set; } = new List<RoleEntitlement>();            

    }
}