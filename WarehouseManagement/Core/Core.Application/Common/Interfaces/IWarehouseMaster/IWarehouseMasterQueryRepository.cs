using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.WarehouseMaster.GetAllWarehouseMaster;
using Core.Application.WarehouseMaster.Queries.GetParentWarehouseMaster;
using Core.Application.WarehouseMaster.Queries.GetWareMasterAutoComplete;

namespace Core.Application.Common.Interfaces.IWarehouseMaster
{
    public interface IWarehouseMasterQueryRepository
    {

        Task<(List<WarehouseMasterDto>, int)> GetAllAsync(int PageNumber, int PageSize, string SearchTerm);

        Task<WarehouseMasterDto> GetByIdAsync(int id);

        Task<bool> ExistsByNameAsync(string warehouseName, int? excludeId = null);

        Task<List<GetWarehouseAutoCompleteDto>> GetWarehouseMasterAutoCompletes(string searchPattern);

        Task<List<GetParentWarehouseDto>> GetParentWarehouseMaster();

        Task<List<WarehouseMasterDto>> GetwarehouseAsync();
    }
}