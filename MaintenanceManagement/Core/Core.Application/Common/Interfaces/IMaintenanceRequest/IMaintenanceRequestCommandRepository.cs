using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMaintenanceRequest
{
    public interface IMaintenanceRequestCommandRepository  
    {
          Task<Core.Domain.Entities.MaintenanceRequest> CreateAsync(Core.Domain.Entities.MaintenanceRequest maintenanceRequest);   

            Task<bool> UpdateAsync(Core.Domain.Entities.MaintenanceRequest maintenanceRequest);

    }
}