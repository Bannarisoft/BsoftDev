using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IActivityCheckListMaster
{
    public interface IActivityCheckListMasterCommandRepository
    {
        Task<Core.Domain.Entities.ActivityCheckListMaster> CreateAsync(Core.Domain.Entities.ActivityCheckListMaster activityCheckListMaster);

        Task<bool> UpdateAsync(int id, Core.Domain.Entities.ActivityCheckListMaster activityCheckListMaster);
           

       Task<int> DeleteAsync(int Id,Core.Domain.Entities.ActivityCheckListMaster activityCheckListMaster);

    }
}