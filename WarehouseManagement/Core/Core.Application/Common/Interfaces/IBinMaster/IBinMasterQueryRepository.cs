using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.BinMaster.Queries.GetAllBinMaster;
using Core.Application.BinMaster.Queries.GetBinMasterAutoComplete;

namespace Core.Application.Common.Interfaces.IBinMaster
{
    public interface IBinMasterQueryRepository
    {

        Task<(List<BinMasterDto>, int)> GetAllAsync(int PageNumber, int PageSize, string SearchTerm);

        Task<BinMasterDto> GetByIdAsync(int id);

        Task<bool> ExistsByWarehouseAndCodeAsync(int warehouseId, string binCode, CancellationToken ct = default);

        Task<IEnumerable<string>> GetBinCodesByPrefixAsync(string prefix, CancellationToken ct = default);
      
          Task<IReadOnlyList<BinAutoDto>> AutocompleteAsync( string? q,int top = 10,  int? warehouseId = null,  int? rackId = null, CancellationToken ct = default);
    }
}