using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using BSOFT.Domain.Common;

namespace BSOFT.Domain.Entities
{
    [Table("Entity", Schema = "AppData")]
    public class Entity : BaseEntity
    {
    public int EntityId { get; set; }
    public string EntityCode { get; set; }
    public string EntityName { get; set; }
    public string EntityDescription { get; set; }
    public string Address { get; set; }
    public string Phone  { get; set; }
    public string Email { get; set; }
    public byte IsActive { get; set; } 
    }
}