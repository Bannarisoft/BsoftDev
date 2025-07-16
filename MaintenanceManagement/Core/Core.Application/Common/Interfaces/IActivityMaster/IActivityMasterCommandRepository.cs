using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.ActivityMaster.Command.UpdateActivityMster;

namespace Core.Application.Common.Interfaces.IActivityMaster
{
    public interface IActivityMasterCommandRepository
    {
        Task<Core.Domain.Entities.ActivityMaster> CreateAsync(Core.Domain.Entities.ActivityMaster  activityMaster); 

      //  Task<Core.Domain.Entities.ActivityMaster> UpdateAsync(Core.Domain.Entities.ActivityMaster activityMaster);  
       //  Task<bool> UpdateAsync( Core.Domain.Entities.ActivityMaster activityMaster);

         Task<int> UpdateAsync(UpdateActivityMasterDto activityMaster);
    }
}