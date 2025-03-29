using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.ActivityMaster.Command.CreateActivityMaster
{
    public class CreateActivityMasterDto
    {
       
        public string? ActivityName { get; set;}
        public string? Description { get; set; }
        public int DepartmentId { get; set; }
       // public string? Department { get; set; } 
        // public int MachineGroupId { get; set; }
        // public string? MachineGroup { get; set; } 
        public int EstimatedDuration { get; set; }
        public int ActivityType { get; set; }
    

        public List<ActivityMachineGroupDto>? ActivityMachineGroup { get; set; }


    }
}