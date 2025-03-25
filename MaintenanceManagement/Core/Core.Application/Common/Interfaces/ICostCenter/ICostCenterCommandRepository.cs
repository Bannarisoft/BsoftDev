using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.ICostCenter
{
    public interface ICostCenterCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.CostCenter costCenter);
        Task<bool> ExistsByCodeAsync(string? costCenterCode);
        Task<int> UpdateAsync(int Id,Core.Domain.Entities.CostCenter costCenter);
        Task<int> DeleteAsync(int Id,Core.Domain.Entities.CostCenter costCenter);
        Task<bool> IsNameDuplicateAsync(string? name, int excludeId);
      
    }
}