using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPartyMaster
{
    public interface IPartyActivityLogCommandRepository
    {
        Task<int> InsertAsync(PartyActivityLog log, CancellationToken cancellationToken = default);
        Task<List<PartyActivityLog>> GetActivityLogsByPartyIdAsync(int partyId, CancellationToken cancellationToken);
    }
}