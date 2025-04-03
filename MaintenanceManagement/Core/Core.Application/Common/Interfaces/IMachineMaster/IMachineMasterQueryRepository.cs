using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IMachineMaster
{
    public interface IMachineMasterQueryRepository
    {
        Task<Core.Domain.Entities.MachineMaster?> GetByIdAsync(int Id);
        Task<(List<Core.Domain.Entities.MachineMaster>,int)> GetAllMachineAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<Core.Domain.Entities.MachineMaster>> GetMachineAsync(string searchPattern);
    }
}