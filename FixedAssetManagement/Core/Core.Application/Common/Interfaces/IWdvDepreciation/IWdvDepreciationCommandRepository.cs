using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;

namespace Core.Application.Common.Interfaces.IWdvDepreciation
{
    public interface IWdvDepreciationCommandRepository
    {               
        Task<int> DeleteAsync(int finYearId);     
        Task<int> LockWDVDepreciationAsync(int finYearId);        
    }
}