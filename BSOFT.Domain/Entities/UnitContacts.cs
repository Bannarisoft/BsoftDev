using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using BSOFT.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace BSOFT.Domain.Entities
{
 
    public class UnitContacts
    {
         public int Id { get; set; }

        public int  UnitId { get; set; }

        
        public string Name { get; set; }

  
        public string Designation { get; set; }

    
        public string Email { get; set; }

        public string PhoneNo { get; set; }

  
        public string? Remarks { get; set; }

        public Unit Unit { get; set; }
    }
}