using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMaintenanceCategory
{
    public interface IMaintenanceCategoryCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.MaintenanceCategory maintenanceCategory);
        Task<int> UpdateAsync(int Id,Core.Domain.Entities.MaintenanceCategory maintenanceCategory);
        Task<int> DeleteAsync(int Id,Core.Domain.Entities.MaintenanceCategory maintenanceCategory);
        Task<bool> IsNameDuplicateAsync(string? name, int excludeId);
    }
}