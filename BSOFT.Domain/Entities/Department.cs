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
    [Table("Department", Schema = "AppData")]
    public class Department : BaseEntity
    {
         [Key]
        public int Id { get; set; }
        [MaxLength(15)]
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CompanyId { get; set; }
        public byte  IsActive { get; set; }
                  

    }
       
   
}