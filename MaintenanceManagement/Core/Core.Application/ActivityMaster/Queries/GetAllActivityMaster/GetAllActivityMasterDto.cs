using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.ActivityMaster.Queries.GetAllActivityMaster
{
    public class GetAllActivityMasterDto
    {
        public int Id { get; set;}
        public string? ActivityName { get; set;}
        public string? Description { get; set; }
        public int DepartmentId { get; set; }
        public string? Department { get; set; } 
        public int MachineGroupId { get; set; }
        public string? MachineGroup { get; set; } 
        public int EstimatedDuration { get; set; }
        public int ActivityType { get; set; }
        public string? ActivityTypeDescription { get; set; }
         public Status  IsActive { get; set; }
        public IsDelete IsDeleted { get; set; }
        
    }
}