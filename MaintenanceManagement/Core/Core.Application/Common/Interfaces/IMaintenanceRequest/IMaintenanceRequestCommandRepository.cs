using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMaintenanceRequest
{
    public interface IMaintenanceRequestCommandRepository  
    {
          Task<int> CreateAsync(Core.Domain.Entities.MaintenanceRequest maintenanceRequest);   

            Task<bool> UpdateAsync(Core.Domain.Entities.MaintenanceRequest maintenanceRequest);

            Task<bool> UpdateStatusAsync(int id);

    }
}