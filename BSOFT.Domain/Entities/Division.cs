using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BSOFT.Domain.Common;

namespace BSOFT.Domain.Entities
{
    [Table("Division", Schema = "AppData")]
    public class Division : BaseEntity
    {
        [Key]
        public int DivId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        
    }
}