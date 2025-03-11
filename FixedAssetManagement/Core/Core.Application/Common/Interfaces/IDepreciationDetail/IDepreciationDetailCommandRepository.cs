using System;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IDepreciationDetail
{
    public interface IDepreciationDetailCommandRepository
    {        
        Task<int> DeleteAsync(int companyId, int unitId, string finYear, string depreciationType,int depreciationPeriod);      
        //Task<int> UpdateAsync(int companyId, int unitId, string finYear, DateTimeOffset startDate, DateTimeOffset endDate,string depreciationType,DepreciationDetails depreciationDetails);      
    }
}
