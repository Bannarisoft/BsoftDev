using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using BSOFT.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace BSOFT.Domain.Entities
{
    [Table("UnitContacts", Schema = "AppData")]
    public class UnitContacts
    {
         public int Id { get; set; }

        public int  UnitId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Designation { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(15)]
        public string PhoneNo { get; set; }

        [MaxLength(250)]
        public string Remarks { get; set; }
    }
}