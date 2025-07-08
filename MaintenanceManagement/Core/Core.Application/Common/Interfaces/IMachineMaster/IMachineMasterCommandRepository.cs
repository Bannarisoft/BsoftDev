using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMachineMaster
{
    public interface IMachineMasterCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.MachineMaster machineMaster);
        Task<bool> UpdateAsync(int Id, Core.Domain.Entities.MachineMaster machineMaster);
        Task<bool> DeleteAsync(int Id, Core.Domain.Entities.MachineMaster machineMaster);
        Task<bool> IsNameDuplicateAsync(string? name,int machineGroupId, int excludeId);
        Task<bool> ExistsByCodeAsync(string? MachineCode);
        Task<bool> IsCodeDuplicateAsync(string? code, int excludeId);
    }
}