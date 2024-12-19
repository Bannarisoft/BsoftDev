using BSOFT.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Entities
{
    [Table("Users", Schema = "AppSecurity")]
    public class User : BaseEntity
    {
    public Guid Id { get; set; }
    public int UserId { get; set; }// Identity column
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public bool IsFirstTimeUser { get; set; } = false;
    public string PasswordHash { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CompanyId { get; set; }
    public int UnitId { get; set; }
    public int DivisionId { get; set; }
    public int UserRoleId { get; set; }
    // public string Role { get; set; }

    }
}