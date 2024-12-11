using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Domain.Entities
{
    [Table("Department", Schema = "AppData")]
    public class Department
    {
         [Key]
        public int DeptId { get; set; }
        public string ShortName { get; set; }
        public string DeptName { get; set; }
        public int CoId { get; set; }
        public byte  IsActive { get; set; }
        public int CreatedBy  { get; set; }
        public DateTime CreatedAt  { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedIP { get; set; }         
        public int ModifiedBy  { get; set; }
        public DateTime ModifiedAt  { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedIP { get; set; }                  

    }
       
   
}