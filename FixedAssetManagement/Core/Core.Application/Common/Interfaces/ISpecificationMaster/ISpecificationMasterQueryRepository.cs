using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ISpecificationMaster
{
    public interface ISpecificationMasterQueryRepository
    {
        Task<SpecificationMasters>  GetByIdAsync(int specId);
        Task<(List<SpecificationMasterDTO>,int)> GetAllSpecificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<SpecificationMasters>> GetBySpecificationNameAsync(int? assetGroupId, string specificationName);  
        Task<bool> SoftDeleteValidation(int Id);      
    }
}