using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Dtos.Party;

namespace Contracts.Interfaces.External.IParty
{
    public interface IPartyGrpcClient
    {
        Task<List<PartyDto>> GetAutoCompleteAsync(string? searchPattern, CancellationToken ct = default);
        Task<PartyDto?> GetByIdAsync(int id, CancellationToken ct = default);        
    }
}