using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class MachineGroup : BaseEntity

    {
        public string?  GroupName { get; set; }
            
        public int Manufacturer { get; set;}       
         public ICollection<ActivityMachineGroup>? ActivityMachineGroups { get; set; }  
        public ICollection<ActivityMaster>? ActivityMasters { get; set; }   
         public ICollection<MachineGroupUser>? MachineGroupUser { get; set; }             
        public ICollection<MachineMaster>? MachineMasters { get; set; }   
        
    }
}