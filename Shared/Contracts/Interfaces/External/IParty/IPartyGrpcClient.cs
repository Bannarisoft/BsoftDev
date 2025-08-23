using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Dtos.Party;

namespace Contracts.Interfaces.External.IParty
{
    public interface IPartyGrpcClient
    {
        Task<List<PartyDto>> GetAllPartyMasterAsync();
    }
}