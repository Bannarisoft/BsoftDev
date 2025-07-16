using Core.Application.WDVDepreciation.Queries.GetDepreciation;

namespace Core.Application.Common.Interfaces.IWdvDepreciation
{
    public interface IWdvDepreciationQueryRepository
    {        
        Task<List<CalculationDepreciationDto>> CreateAsync(int finYearId);        
        Task<List<CalculationDepreciationDto>> GetWDVDepreciationAsync(int finYearId);        
        Task<bool> ExistDataAsync(int finYearId);
        Task<bool> ExistDataLockedAsync(int finYearId);        
    }
}