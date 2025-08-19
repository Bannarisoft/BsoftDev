using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.RackMaster.Queries.GetAllRackMaster;
using Core.Application.RackMaster.Queries.GetRackMasterAutoComplete;

namespace Core.Application.Common.Interfaces.IRackMaster
{
    public interface IRackMasterQueryRepository
    {
        Task<(List<RackMasterDto>, int)> GetAllAsync(int PageNumber, int PageSize, string SearchTerm);
        Task<RackMasterDto> GetByIdAsync(int id);
        Task<bool> RackSlotAlreadyExistsAsync(int warehouseId, int? floorId, int? aisleId, int? rackLevelId, int? id = null);
        
        Task<List<GetRackMasterAutoCompleteDto>>  GetRackMasterAutoCompletes(string searchPattern);
    }
}