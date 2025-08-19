using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IWarehouseMaster
{
    public interface IWarehouseMasterCommandRepository
    {
        Task<int> CreateAsync(Domain.Entities.WarehouseMaster warehouseMaster);
        Task<int> UpdateAsync(Domain.Entities.WarehouseMaster warehouseMaster);
        Task<bool> DeleteAsync( int id, Domain.Entities.WarehouseMaster warehouseMaster);

        Task<Core.Domain.Entities.WarehouseMaster?> GetByIdAsync(int id);


    }
}