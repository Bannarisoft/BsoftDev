using Core.Domain.Entities;

namespace Core.Application.Common.Interfaces.ISpecificationMaster
{
    public interface ISpecificationMasterCommandRepository
    {
        Task<SpecificationMasters> CreateAsync(SpecificationMasters specificationMaster);
        Task<int>  UpdateAsync(int specId,SpecificationMasters specificationMaster);
        Task<int>  DeleteAsync(int specId,SpecificationMasters specificationMaster); 
        Task<bool> ExistsByAssetGroupIdAsync(int? assetGroupId,string? specificationName); // ✅ New method       
    }
}