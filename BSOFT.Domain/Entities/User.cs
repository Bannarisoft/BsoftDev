using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Domain.Entities
{
    public class User
    {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public byte IsActive { get; set; }
    public string UserPassword { get; set; }
    public int UserType { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime Created_Time { get; set; }
    public string? CreatedByName { get; set; }
    public int ModifiedBy { get; set; }
    public DateTime Modified_Time { get; set; }
    public string? ModifiedByName { get; set; }
       
    }
}