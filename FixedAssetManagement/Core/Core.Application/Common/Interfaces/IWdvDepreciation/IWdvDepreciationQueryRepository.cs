using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;

namespace Core.Application.Common.Interfaces.IWdvDepreciation
{
    public interface IWdvDepreciationQueryRepository
    {
        Task<List<CalculationDepreciationDto>> CalculateWDVAsync(int finYearId);
        Task<List<CalculationDepreciationDto>> CreateAsync(int finYearId);        
        Task<List<CalculationDepreciationDto>> GetWDVDepreciationAsync(int finYearId);        
        Task<bool> ExistDataAsync(int finYearId);
        Task<bool> ExistDataLockedAsync(int finYearId);        
    }
}