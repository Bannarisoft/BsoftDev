
using Core.Application.DepreciationCalculation.Queries.GetDepreciationCalculation;

namespace Core.Application.Common.Interfaces.IDepreciationCalculation
{
    public interface IDepreciationCalculationQueryRepository
    {
        Task<DepreciationDto>  GetByIdAsync(int CompanyId,int UnitId, DateTimeOffset StartDate,DateTimeOffset EndDate, int PageNumber, int PageSize, string? SearchTerm,int assetId);
        Task<(List<DepreciationDto>,int)> CalculateDepreciationAsync(int CompanyId,int UnitId, DateTimeOffset StartDate,DateTimeOffset EndDate, int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<DepreciationDto>> GetByAssetNameAsync(string assetName);    
    }
}