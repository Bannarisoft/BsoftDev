using System;
using System.Threading.Tasks;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.IDepreciationDetail
{
    public interface IDepreciationDetailCommandRepository
    {        
        Task<int> DeleteAsync(int companyId, int unitId, int finYearId, int depreciationType,int depreciationPeriod);      
        Task<int> UpdateAsync(int companyId, int unitId, int finYearId, int depreciationType,int depreciationPeriod);   
        
    }
}
