using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ISpecificationMaster
{
    public interface ISpecificationMasterQueryRepository
    {
        Task<SpecificationMasters>  GetByIdAsync(int specId);
        Task<(List<SpecificationMasters>,int)> GetAllSpecificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm);        
        Task<List<SpecificationMasters>> GetBySpecificationNameAsync(string specificationName);        
    }
}