using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyMaster.Queries.GetPartyDetails;

namespace Core.Application.Common.Interfaces.IPartyMaster
{
    public interface IPartyMasterQueryRepository
    {
        Task<List<PartyMasterDTO>> GetPartyMasters(string OldunitCode,string searchPattern);
    }
}