using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMaintenanceCategory
{
    public interface IMaintenanceCategoryQueryRepository
    {
        Task<Core.Domain.Entities.MaintenanceCategory?> GetByIdAsync(int Id);
        Task<(List<Core.Domain.Entities.MaintenanceCategory>,int)> GetAllMaintenanceCategoryAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<Core.Domain.Entities.MaintenanceCategory>> GetMaintenanceCategoryAsync(string searchPattern);
    }
}