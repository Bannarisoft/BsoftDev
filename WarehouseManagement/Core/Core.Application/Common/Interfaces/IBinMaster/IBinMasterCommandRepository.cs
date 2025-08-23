using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IBinMaster
{
    public interface IBinMasterCommandRepository
    {
        Task<int> CreateAsync(Domain.Entities.BinMaster binMaster);

        Task<Core.Domain.Entities.BinMaster?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> UpdateAsync(Core.Domain.Entities.BinMaster entity, CancellationToken ct = default);
        
        Task<int> DeleteAsync(int id , CancellationToken ct = default);
    }
}       