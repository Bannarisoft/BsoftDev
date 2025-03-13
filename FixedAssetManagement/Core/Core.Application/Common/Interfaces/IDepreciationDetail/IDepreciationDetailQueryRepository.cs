
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;

namespace Core.Application.Common.Interfaces.IDepreciationDetail
{
    public interface IDepreciationDetailQueryRepository
    {         
        Task<List<DepreciationDto>> CreateAsync(int companyId, int unitId, string finYear, int depreciationType,int depreciationPeriod);         
        Task<(List<DepreciationDto>,int)> CalculateDepreciationAsync(int companyId,int unitId,string finYear, DateTimeOffset? startDate,DateTimeOffset? endDate,int DepreciationType, int PageNumber, int PageSize, string? SearchTerm,int depreciationPeriod);                        
        Task<bool> ExistDataAsync(int companyId, int unitId, string finYear, int depreciationType,int depreciationPeriod);
        Task<bool> ExistDataLockedAsync(int companyId, int unitId, string finYear, int depreciationType,int depreciationPeriod);
        Task<List<DepreciationAbstractDto>> GetDepreciationAbstractAsync (int companyId, int unitId, string finYear,int depreciationPeriod, int depreciationType);
        Task<List<Core.Domain.Entities.MiscMaster>> GetDepreciationPeriodAsync();   
        Task<List<Core.Domain.Entities.MiscMaster>> GetDepreciationMethodAsync();   
    }
}