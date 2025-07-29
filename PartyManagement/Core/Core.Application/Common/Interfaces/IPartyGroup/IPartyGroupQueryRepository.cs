using Core.Application.PartyGroup.Queries.GetPartyGroup;
using Core.Application.PartyGroup.Queries.GetPartyGroupAutoComplete;
using Core.Application.PartyGroup.Queries.GetPartyGroupById;

namespace Core.Application.Common.Interfaces.IPartyGroup
{
    public interface IPartyGroupQueryRepository
    {
        Task<PartyGroupByIdDto?> GetByIdAsync(int Id);
        Task<(List<PartyGroupDto>, int)> GetAllPartyGroupAsync(int PageNumber, int PageSize, string? SearchTerm);
        Task<List<PartyGroupAutoCompleteDto>> GetMainPartyGroups(string searchPattern);
        Task<List<PartyGroupAutoCompleteDto>> GetParentPartyGroups(string searchPattern);
    }
}