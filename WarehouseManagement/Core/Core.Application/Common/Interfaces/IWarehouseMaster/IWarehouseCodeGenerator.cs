using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IWarehouseMaster
{
    public interface IWarehouseCodeGenerator
    {
        Task<string> GenerateAsync(int unitId, int warehouseTypeId);
    }
}