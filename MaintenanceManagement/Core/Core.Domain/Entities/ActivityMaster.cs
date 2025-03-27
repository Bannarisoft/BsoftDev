using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class ActivityMaster : BaseEntity
    {      
      public string? ActivityCode { get; set; }
      public string? ActivityName { get; set; }
      public string? Description { get; set; }
      public int DepartmentId { get; set; }
      public int MachineGroup { get; set; }
      public decimal EstimatedDuration { get; set; }
      public string? ActivityType { get; set; }    
        
    }
}