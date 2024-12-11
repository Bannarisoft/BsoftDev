using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BSOFT.Domain.Entities
{
    [Table("Division", Schema = "AppData")]
    public class Division
    {
        [Key]
        public int DivId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }
    }
}