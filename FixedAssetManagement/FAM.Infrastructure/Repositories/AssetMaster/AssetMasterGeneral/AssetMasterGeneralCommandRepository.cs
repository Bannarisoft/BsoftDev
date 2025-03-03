using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetMasterGeneral
{
    public class AssetMasterGeneralCommandRepository : IAssetMasterGeneralCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;        
        public AssetMasterGeneralCommandRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;            
        }
        public async Task<AssetMasterGenerals> CreateAsync(AssetMasterGenerals assetMasterGeneral, CancellationToken cancellationToken)
        {
           var entry =_applicationDbContext.Entry(assetMasterGeneral);
            await _applicationDbContext.AssetMasterGenerals.AddAsync(assetMasterGeneral);
            await _applicationDbContext.SaveChangesAsync();
             return assetMasterGeneral;   
        }
        public async Task<int> DeleteAsync(int Id, AssetMasterGenerals assetMaster)
        {
            var assetMasterToDelete = await _applicationDbContext.AssetMasterGenerals.FirstOrDefaultAsync(u => u.Id == Id);
            if (assetMasterToDelete != null)
            {
                assetMasterToDelete.IsDeleted = assetMaster.IsDeleted;              
                return await _applicationDbContext.SaveChangesAsync();
            }
            return 0;
        }
        public async Task<int> UpdateAsync(int Id, AssetMasterGenerals assetMaster)
        {
            var existingDepGroup = await _applicationDbContext.AssetMasterGenerals.FirstOrDefaultAsync(u => u.Id == Id);    
            if (existingDepGroup != null)
            {                
                existingDepGroup.AssetName = assetMaster.AssetName;                
                existingDepGroup.AssetGroupId = assetMaster.AssetGroupId;
                existingDepGroup.IsActive = assetMaster.IsActive;
                existingDepGroup.AssetGroupId = assetMaster.AssetGroupId;
                existingDepGroup.AssetCategoryId = assetMaster.AssetCategoryId;
                existingDepGroup.AssetSubCategoryId = assetMaster.AssetSubCategoryId;
                existingDepGroup.AssetParentId = assetMaster.AssetParentId;
                existingDepGroup.AssetType = assetMaster.AssetType;
                existingDepGroup.MachineCode = assetMaster.MachineCode;
                existingDepGroup.Quantity = assetMaster.Quantity;
                existingDepGroup.UOMId = assetMaster.UOMId;
                existingDepGroup.AssetDescription = assetMaster.AssetDescription;
                existingDepGroup.WorkingStatus = assetMaster.WorkingStatus;
                existingDepGroup.AssetImage = assetMaster.AssetImage;
                existingDepGroup.ISDepreciated = assetMaster.ISDepreciated;
                existingDepGroup.IsTangible = assetMaster.IsTangible;
                _applicationDbContext.AssetMasterGenerals.Update(existingDepGroup);
                return await _applicationDbContext.SaveChangesAsync();
            }
           return 0; 
        }     
        public async Task<AssetMasterGenerals?> GetByAssetCodeAsync(string assetCode)
        {
            return await _applicationDbContext.AssetMasterGenerals
                .FirstOrDefaultAsync(a => a.AssetCode == assetCode && a.IsDeleted == BaseEntity.IsDelete.NotDeleted && a.IsActive == BaseEntity.Status.Active );   
        }
        public async Task<bool> UpdateAssetImageAsync(int assetId, string imageName)
        {
            var asset = await _applicationDbContext.AssetMasterGenerals.FindAsync(assetId);
            if (asset == null)
            {
                return false;  // Asset not found
            }
            // Store only relative path (e.g., "HomeTextile/HomeTextile-COMP-MOU-1.png")
            asset.AssetImage = imageName.Replace(@"\", "/"); 

            asset.AssetImage = imageName;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<AssetMasterGeneralDTO?> GetByAssetImageAsync(string assetCode)
        {
           /*  return await _applicationDbContext.AssetMasterGenerals
                .FirstOrDefaultAsync(a => a.AssetImage == assetCode); */
            return await _applicationDbContext.AssetMasterGenerals
            .Where(a => a.AssetCode == assetCode)
            .Select(a => new AssetMasterGeneralDTO
            {
                AssetCode = a.AssetCode,
                AssetImage = a.AssetImage,
                Id = a.Id,
            })
            .FirstOrDefaultAsync();
        }

        public async Task<bool> RemoveAssetImageReferenceAsync(int assetId)
        {
            var asset = await _applicationDbContext.AssetMasterGenerals.FindAsync(assetId);
            if (asset == null)
            {
                return false;  // Asset not found
            }

            asset.AssetImage = null;
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }    
    }
}