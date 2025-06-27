
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.ExcelImport.PhysicalStockVerification;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Interfaces.IExcelImport
{
    public interface IExcelImportCommandRepository
    {
        Task AddRangeAsync(IEnumerable<AssetMasterGenerals> assets);
        Task SaveChangesAsync();
        Task<bool> ImportAssetsAsync(List<AssetMasterDto> assets, CancellationToken cancellationToken);
        Task<int?> GetAssetGroupIdByNameAsync(string assetGroupName);
        Task<int?> GetAssetCategoryIdByNameAsync(string assetGroupName);
        Task<int?> GetAssetSubCategoryIdByNameAsync(int assetCategoryId,string assetSubGroupName);
        Task<int?> GetAssetUOMIdByNameAsync(string assetGroupName);
        Task<int?> GetAssetLocationIdByNameAsync(string locationName);
        Task<int?> GetAssetSubLocationIdByNameAsync(string subLocationName, string locationName);
        Task<int?> GetAssetIdByNameAsync(string assetCode);
        Task<int?> GetManufacturerIdByNameAsync(string manufacture);
        Task<bool> BulkInsertAsync(List<AssetAudit> audits, CancellationToken cancellationToken);
        Task<bool> CheckFileExistsAsync(string fileName, CancellationToken cancellationToken);
        Task<bool> InsertScannedAssetAsync(AssetAudit entity, CancellationToken cancellationToken);
        Task<bool> IsAssetAlreadyScannedAsync(
        string assetCode,
        int auditCycle,
        string auditFinancialYear,
        string department,
        string UnitName,
        CancellationToken cancellationToken);
    
        
    }
}