using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Domain.Entities
{
    public class ActivityMaster : BaseEntity
    {      
     
      public string? ActivityName { get; set; }
      public string? Description { get; set; }
      public int DepartmentId { get; set; }
     // public int MachineGroupId { get; set; }
      public int EstimatedDuration { get; set; }
      public int ActivityType { get; set; } 
      public ICollection<ActivityMachineGroup>? ActivityMachineGroups { get; set; } // ✅ Many-to-Many Relation 
      public ICollection<WorkOrderActivity>? workOrderActivities { get; set; } 

    }
}