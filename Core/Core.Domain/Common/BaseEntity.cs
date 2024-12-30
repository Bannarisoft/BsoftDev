using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Core.Domain.Common
{
    public abstract class BaseEntity
    {
        public byte  IsActive { get; set; }=1;
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