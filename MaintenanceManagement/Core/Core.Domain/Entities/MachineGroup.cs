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
        public string? Manufacturer { get; set;}            
        
    }
}