using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IMiscMaster
{
  public interface IMiscMasterQueryRepository
  {
    Task<(List<Core.Domain.Entities.MiscMaster>, int)> GetAllMiscMasterAsync(int PageNumber, int PageSize, string? SearchTerm);
    Task<Core.Domain.Entities.MiscMaster> GetByIdAsync(int id);
    //  Task<List<Core.Domain.Entities.MiscMaster>> GetMiscMaster(string searchPattern);

    Task<List<Core.Domain.Entities.MiscMaster>> GetMiscMaster(string miscTypeCode, string miscTypeName);

    // Task<Core.Domain.Entities.MiscMaster?> GetByMiscMasterCodeAsync(string name,int? id = null);

    Task<Core.Domain.Entities.MiscMaster?> GetByMiscMasterCodeAsync(string name, int miscTypeId, int? id = null);

    Task<bool> AlreadyExistsAsync(string code, int miscTypeId, int? id = null);
     
    Task<Core.Domain.Entities.MiscMaster?> GetByMiscTypeIdAndCodeAsync(int miscTypeId, string code);
            
    }
}