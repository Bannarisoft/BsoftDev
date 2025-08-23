using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPartyMaster
{
    public interface IPartyMasterCommandRepository
    {
        Task<int> CreateAsync(Core.Domain.Entities.PartyMaster partyMaster);
        Task<bool> UpdateAsync(int Id, Core.Domain.Entities.PartyMaster partyMaster);
        Task<bool> DeleteAsync(int Id, Core.Domain.Entities.PartyMaster partyMaster);
        Task<string> GetNextPartyCodeAsync();
        Task<bool> DeleteFileDetailsDocumentAsync(int Id, int PartyId, string filename);
        Task<List<int>> GetPartyDocumentIdsAsync(int partyId);
        Task<bool> LogChange(int partyId, string tableName, string columnName, string oldValue, string newValue,string actionType);
        
    }
}