using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IDepreciationGroup
{
    public interface IDepreciationGroupQueryRepository
    {
        Task<DepreciationGroupDTO>  GetByIdAsync(int depGroupId);
        Task<(List<DepreciationGroupDTO>,int)> GetAllDepreciationGroupAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<DepreciationGroupDTO>> GetByDepreciationNameAsync(string depreciationGroupName);             
        Task<List<Core.Domain.Entities.MiscMaster>> GetBookTypeAsync();             
        Task<List<Core.Domain.Entities.MiscMaster>> GetDepreciationMethodAsync();        

    }
}