using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IDepreciationGroup
{
    public interface IDepreciationGroupQueryRepository
    {
        Task<DepreciationGroups>  GetByIdAsync(int depGroupId);
        Task<(List<DepreciationGroups>,int)> GetAllDepreciationGroupAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<DepreciationGroups>> GetByDepreciationNameAsync(string depreciationGroupName);                 
    }
}