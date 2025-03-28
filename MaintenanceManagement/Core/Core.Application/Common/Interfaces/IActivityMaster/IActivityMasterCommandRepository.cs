using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IActivityMaster
{
    public interface IActivityMasterCommandRepository
    {
        Task<Core.Domain.Entities.ActivityMaster> CreateAsync(Core.Domain.Entities.ActivityMaster  activityMaster); 
    }
}