using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.ActivityMaster.Command.UpdateActivityMster
{
    public class UpdateActivityMasterDto
    {
        public int ActivityId { get; set;}
        public string? ActivityName { get; set;}
        public string? Description { get; set; }
        public int DepartmentId { get; set; }
        public int EstimatedDuration { get; set; }
        public int ActivityType { get; set; }
    

        public List<UpdateActivityMachineGroupDto>? UpdateActivityMachineGroup  { get; set; }
        
    }
}