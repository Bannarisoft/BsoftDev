using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.PartyMaster.Queries.GetPartMaster;
using Core.Application.PartyMaster.Queries.GetPartMasterAutoComplete;
using Core.Application.PartyMaster.Queries.GetPartyGroupLoad;
using Core.Application.PartyMaster.Queries.GetPartyMasterById;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IPartyMaster
{
    public interface IPartyMasterQueryRepository
    {
        Task<List<PartyGroupLoadDto>> GetPartyGroupsAsync(List<int> groupTypeIds);
        Task<string> GetDocumentDirectoryAsync();
        Task<string> GetBaseDirectoryAsync();
        Task<PartyMasterDto> GetByIdPartyMasterAsync(int id);
        Task<(List<GetPartyMasterDto>, int)> GetAllPartyMasterAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<GetPartyMasterAutoCompleteDto>> GetPartyMasterAutoComplete(string searchPattern);
        Task<List<GetPartyMasterAutoCompleteDto>> GetByIdsAsync(IEnumerable<int> ids);      
    }
}