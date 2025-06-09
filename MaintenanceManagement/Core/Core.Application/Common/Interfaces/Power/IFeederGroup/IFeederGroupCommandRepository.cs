using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.Power.IFeederGroup
{
    public interface IFeederGroupCommandRepository
    {

        Task<int> CreateAsync(Core.Domain.Entities.Power.FeederGroup feederGroup);

        Task<bool> UpdateAsync(int id, Core.Domain.Entities.Power.FeederGroup feederGroup);
        
         Task<bool> DeleteAsync(int id,Core.Domain.Entities.Power.FeederGroup feederGroup); 


        
    }
}