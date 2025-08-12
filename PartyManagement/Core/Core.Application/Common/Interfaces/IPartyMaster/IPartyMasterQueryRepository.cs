using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyMaster.Queries.GetPartyGroupLoad;

namespace Core.Application.Common.Interfaces.IPartyMaster
{
    public interface IPartyMasterQueryRepository
    {
        Task<List<PartyGroupLoadDto>> GetPartyGroupsAsync(List<int> groupTypeIds);
    }
}