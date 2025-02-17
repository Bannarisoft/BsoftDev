using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IMiscMaster
{
    public interface IMiscMasterQueryRepository
    {
         Task<(List<Core.Domain.Entities.MiscMaster>,int)> GetAllMiscMasterAsync(int PageNumber, int PageSize, string? SearchTerm);
         Task<Core.Domain.Entities.MiscMaster> GetByIdAsync(int id);
         Task<List<Core.Domain.Entities.MiscMaster>> GetMiscMaster(string searchPattern);
         Task<Core.Domain.Entities.MiscMaster?> GetByMiscMasterCodeAsync(string name,int? id = null);

            
    }
}