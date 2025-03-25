using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMachineGroup
{
    public interface IMachineGroupCommandRepository 
    {

      Task<Core.Domain.Entities.MachineGroup> CreateAsync(Core.Domain.Entities.MachineGroup machineGroup); 
      Task<bool> UpdateAsync(int id, Core.Domain.Entities.MachineGroup machineGroup);  
       Task<bool> DeleteAsync(int id,Core.Domain.Entities.MachineGroup  machineGroup); 
  
            
    }
}