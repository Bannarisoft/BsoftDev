using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.MachineGroup.Queries.GetMachineGroup
{
    public class MachineGroupDto 
    {        
        public int Id { get; set; }
        public string? GroupName { get; set; }  
        public int Manufacturer  { get; set;}
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public Status IsActive { get; set; }
        public IsDelete IsDeleted { get; set; } 

    }
}