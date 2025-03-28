using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMaintenanceType
{
    public interface IMaintenanceTypeCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.MaintenanceType maintenanceType);
        Task<int> UpdateAsync(int Id,Core.Domain.Entities.MaintenanceType maintenanceType);
        Task<int> DeleteAsync(int Id,Core.Domain.Entities.MaintenanceType maintenanceType);
        Task<bool> IsNameDuplicateAsync(string? name, int excludeId);
        Task<bool> ExistsByCodeAsync(string? TypeName);
    }
}