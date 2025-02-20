using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetSpecification
{
    public class AssetSpecificationCommandRepository : IAssetSpecificationCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;        
        public AssetSpecificationCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;            
        }
        public async Task<AssetSpecifications> CreateAsync(AssetSpecifications assetSpecifications)
        {            
            await _applicationDbContext.AssetSpecifications.AddAsync(assetSpecifications);
            await _applicationDbContext.SaveChangesAsync();
            return assetSpecifications;          
        }
        public async Task<int> DeleteAsync(int depGroupId, AssetSpecifications assetSpecifications)
        {
            var assetSpecToDelete = await _applicationDbContext.AssetSpecifications.FirstOrDefaultAsync(u => u.Id == depGroupId);
            if (assetSpecToDelete != null)
            {
                assetSpecToDelete.IsDeleted = assetSpecifications.IsDeleted;              
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0;
        }
        public async Task<int> UpdateAsync(int assetId, AssetSpecifications assetSpecifications)
        {
            var existingAssetSpecGroup = await _applicationDbContext.AssetSpecifications.FirstOrDefaultAsync(u => u.Id == assetId);             
    
            if (existingAssetSpecGroup != null)
            {
                existingAssetSpecGroup.AssetId = assetSpecifications.AssetId;
                existingAssetSpecGroup.ManufactureId = assetSpecifications.ManufactureId;                
                existingAssetSpecGroup.ManufactureDate = assetSpecifications.ManufactureDate;
                existingAssetSpecGroup.IsActive = assetSpecifications.IsActive;
                existingAssetSpecGroup.SpecificationId = assetSpecifications.SpecificationId;
                existingAssetSpecGroup.SpecificationValue = assetSpecifications.SpecificationValue;
                existingAssetSpecGroup.SerialNumber = assetSpecifications.SerialNumber;
                existingAssetSpecGroup.ModelNumber = assetSpecifications.ModelNumber;
                _applicationDbContext.AssetSpecifications.Update(existingAssetSpecGroup);
                return await _applicationDbContext.SaveChangesAsync();
            }
           return 0; 
        }
        public async Task<bool> ExistsByAssetSpecIdAsync(int? assetId,int? assetSpecId)
        {
            return await _applicationDbContext.AssetSpecifications.AnyAsync(c => c.AssetId == assetId && c.SpecificationId == assetSpecId && c.IsDeleted==BaseEntity.IsDelete.NotDeleted );
        }

        public async Task<bool> ExistsByManufactureAsync(int? manufactureId)
        {
            return await _applicationDbContext.Manufactures.AnyAsync(c => c.Id == manufactureId && c.IsDeleted==BaseEntity.IsDelete.NotDeleted && c.IsActive==BaseEntity.Status.Active);
        }
    }
}