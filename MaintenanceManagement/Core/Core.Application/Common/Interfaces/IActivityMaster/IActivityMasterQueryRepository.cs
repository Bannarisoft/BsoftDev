using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IActivityMaster
{
    public interface IActivityMasterQueryRepository
    {

          Task<(List<Core.Domain.Entities.ActivityMaster>,int)> GetAllActivityMasterAsync(int PageNumber, int PageSize, string? SearchTerm);

           Task<Core.Domain.Entities.ActivityMaster> GetByIdAsync(int id);

           Task<List<Core.Domain.Entities.ActivityMaster>> GetActivityMasterAutoComplete(string searchPattern);


            Task<bool> GetByActivityNameAsync(string activityname);
        
    }
}