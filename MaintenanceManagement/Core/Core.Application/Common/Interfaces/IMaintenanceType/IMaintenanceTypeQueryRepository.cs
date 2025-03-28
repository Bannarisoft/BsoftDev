using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMaintenanceType
{
    public interface IMaintenanceTypeQueryRepository
    {
         Task<Core.Domain.Entities.MaintenanceType?> GetByIdAsync(int Id);
         Task<(List<Core.Domain.Entities.MaintenanceType>,int)> GetAllMaintenanceTypeAsync(int PageNumber, int PageSize, string? SearchTerm);
         Task<List<Core.Domain.Entities.MaintenanceType>> GetMaintenanceTypeAsync(string searchPattern);
    }
}