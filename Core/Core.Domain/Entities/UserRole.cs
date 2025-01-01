using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    [Table("UserRole", Schema = "AppSecurity")]
    public class UserRole  : BaseEntity
    {        
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
        // public UserRole Role { get; set; }
         // Foreign Key to User
        public int UserId { get; set; } 
        public User User { get; set; } // Navigation property to User
       
        //  public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<RoleEntitlement> RoleEntitlements { get; set; } = new List<RoleEntitlement>();       

    }
}