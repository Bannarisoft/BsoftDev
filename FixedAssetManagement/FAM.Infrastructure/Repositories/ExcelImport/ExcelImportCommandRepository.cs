using System.Data;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces.IExcelImport;
using Core.Domain.Entities;
using FAM.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FAM.Infrastructure.Repositories.ExcelImport
{
    public class ExcelImportCommandRepository  : IExcelImportCommandRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;       
        private readonly IMediator _mediator;
        public ExcelImportCommandRepository(ApplicationDbContext applicationDbContext, IMediator mediator)
        {
        _applicationDbContext = applicationDbContext;
         _mediator = mediator;           
        }

        public async Task AddRangeAsync(IEnumerable<AssetMasterGenerals> assets)
        {
            await _applicationDbContext.AssetMasterGenerals.AddRangeAsync(assets);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        public async Task<bool> ImportAssetsAsync(List<AssetMasterDto> assetDto, CancellationToken cancellationToken)
        {
            var strategy = _applicationDbContext.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        foreach (var assetDto in assetDto)
                        {
                            var command = new CreateAssetMasterGeneralCommand { AssetMaster = assetDto };

                            var response = await _mediator.Send(command, cancellationToken);

                            if (response == null || !response.IsSuccess)
                            {
                                throw new Exception($"Error creating asset '{assetDto.AssetName}' at row.");
                            }
                        }

                        await transaction.CommitAsync(cancellationToken);
                        return true;
                    }
                    catch (DbUpdateException dbEx) // 🔹 Capture EF Core Database Errors
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw new Exception($"Database Transaction Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);          
                        throw new Exception($"Unexpected Error: {ex.Message}");
                    }
                }
            });
        }
        // ✅ CreateAsync - Handles Single Asset Insert
        public async Task<AssetMasterGenerals?> CreateAsync(AssetMasterGenerals asset)
        {
            await _applicationDbContext.AssetMasterGenerals.AddAsync(asset);
            await _applicationDbContext.SaveChangesAsync();
            return asset; 
        }
        public async Task<int?> GetAssetGroupIdByNameAsync(string assetGroupName)
        {
            var trimmedAssetGroupName = assetGroupName?.Trim(); // ✅ Trim input
            var assetGroup = await _applicationDbContext.AssetGroup
                .Where(a => a.GroupName.Trim() == trimmedAssetGroupName && a.IsDeleted == 0) // ✅ Trim from DB column for safety
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            return assetGroup == 0 ? null : assetGroup; 
        }

        public async Task<int?> GetAssetCategoryIdByNameAsync(string assetCategoryName)
        {
           var assetCategory = await _applicationDbContext.AssetCategories
            .Where(a => a.CategoryName == assetCategoryName  && a.IsDeleted == 0)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetCategory == 0 ? null : assetCategory;
        }

        public async Task<int?> GetAssetSubCategoryIdByNameAsync(string assetSubCategoryName)
        {
            var assetSubCategory = await _applicationDbContext.AssetSubCategories
            .Where(a => a.SubCategoryName == assetSubCategoryName  && a.IsDeleted == 0) 
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetSubCategory == 0 ? null : assetSubCategory; 
        }

        public async Task<int?> GetAssetUOMIdByNameAsync(string assetUOMName)
        {
            var assetUOM = await _applicationDbContext.UOMs
            .Where(a => a.UOMName == assetUOMName  && a.IsDeleted == 0)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetUOM == 0 ? null : assetUOM;
        }

        public async Task<int?> GetAssetLocationIdByNameAsync(string locationName)
        {
            locationName=locationName.Trim();
            var assetLocation = await _applicationDbContext.Locations
            .Where(a => a.LocationName == locationName  && a.IsDeleted == 0)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetLocation == 0 ? null : assetLocation; 
        }

        public async Task<int?> GetAssetSubLocationIdByNameAsync(string subLocationName)
        {
            subLocationName=subLocationName.Trim();
            var assetSubLocation= await _applicationDbContext.SubLocations
            .Where(a => a.SubLocationName == subLocationName  && a.IsDeleted == 0)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetSubLocation == 0 ? null : assetSubLocation; 
        }

        public async Task<int?> GetAssetIdByNameAsync(string assetCode)
        {
            var assetMaster = await _applicationDbContext.AssetMasterGenerals
            .Where(a => a.AssetCode == assetCode  && a.IsDeleted == 0)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetMaster == 0 ? null : assetMaster;
        }

        public async Task<int?> GetManufacturerIdByNameAsync(string manufacture)
        {
            var assetManufacturer = await _applicationDbContext.Manufactures
            .Where(a => a.ManufactureName == manufacture  && a.IsDeleted == 0)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();        
            return assetManufacturer == 0 ? null : assetManufacturer; 
        }
    }
}