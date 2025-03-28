using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class ActivityMaster : BaseEntity
    {      
     
      public string? ActivityName { get; set; }
      public string? Description { get; set; }
      public int DepartmentId { get; set; }
      public int MachineGroupId { get; set; }
      public int EstimatedDuration { get; set; }
      public int ActivityType { get; set; }    

      public MachineGroup? MachineGroup { get; set; }    
    }
}