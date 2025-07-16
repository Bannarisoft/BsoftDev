using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IWorkCenter
{
    public interface IWorkCenterCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.WorkCenter workCenter);
        Task<bool> ExistsByCodeAsync(string? WorkCenterCode);
        Task<int> UpdateAsync(int Id,Core.Domain.Entities.WorkCenter workCenter);
        Task<int> DeleteAsync(int Id,Core.Domain.Entities.WorkCenter workCenter);
        Task<bool> IsNameDuplicateAsync(string? name, int excludeId);
    }
}