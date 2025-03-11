
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;

namespace Core.Application.Common.Interfaces.IDepreciationDetail
{
    public interface IDepreciationDetailQueryRepository
    {       
        //Task<List<DepreciationDto>> CreateAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate,string depreciationType);         
        Task<List<DepreciationDto>> CreateAsync(int companyId, int unitId, string finYear, string depreciationType,int depreciationPeriod);         
        Task<(List<DepreciationDto>,int)> CalculateDepreciationAsync(int companyId,int unitId,string finYear, DateTimeOffset? startDate,DateTimeOffset? endDate,string DepreciationType, int PageNumber, int PageSize, string? SearchTerm,int depreciationPeriod);                        
        Task<bool> ExistDataAsync(int companyId, int unitId, string finYear, string depreciationType,int depreciationPeriod);
        //Task<bool> DeleteLockedAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate,string depreciationType);
        Task<List<Core.Domain.Entities.MiscMaster>> GetDepreciationPeriodAsync();   
           Task<List<Core.Domain.Entities.MiscMaster>> GetDepreciationMethodAsync();   
    }
}