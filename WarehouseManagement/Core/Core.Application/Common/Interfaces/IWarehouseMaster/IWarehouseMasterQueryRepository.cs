using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;

namespace Core.Application.Common.Interfaces.IWarehouseMaster
{
    public interface IWarehouseMasterQueryRepository
    {

        Task<(List<WarehouseMasterDto>, int)> GetAllAsync(int PageNumber, int PageSize, string SearchTerm);
        
        Task<WarehouseMasterDto> GetByIdAsync(int id);
    }
}