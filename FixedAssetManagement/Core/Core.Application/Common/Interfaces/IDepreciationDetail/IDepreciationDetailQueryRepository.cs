
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;

namespace Core.Application.Common.Interfaces.IDepreciationDetail
{
    public interface IDepreciationDetailQueryRepository
    {       
        Task<string> CreateAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate,string depreciationType);         
        Task<(List<DepreciationDto>,int)> CalculateDepreciationAsync(int companyId,int unitId,string finYear, DateTimeOffset startDate,DateTimeOffset endDate,string DepreciationType, int PageNumber, int PageSize, string? SearchTerm);                
        Task<bool> ExistDataAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate,string depreciationType);
    }
}