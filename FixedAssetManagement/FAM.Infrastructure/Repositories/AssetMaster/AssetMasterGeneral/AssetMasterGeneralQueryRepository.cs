using System.Data;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetMasterGeneral
{
    public class AssetMasterGeneralQueryRepository : IAssetMasterGeneralQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public AssetMasterGeneralQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
        public async Task<(List<AssetMasterGeneralDTO>, int)> GetAllAssetAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", PageNumber);
            parameters.Add("@PageSize", PageSize);
            parameters.Add("@SearchTerm", string.IsNullOrEmpty(SearchTerm) ? null : SearchTerm);
            using var multiResult = await _dbConnection.QueryMultipleAsync(
            "dbo.FAM_GetAllAssets", parameters, commandType: CommandType.StoredProcedure);            
            // Read the first result set (Paginated Asset List)
            var assetMasterList = (await multiResult.ReadAsync<AssetMasterGeneralDTO>()).ToList();
            // Read the second result set (Total Record Count)
            int totalCount = await multiResult.ReadFirstAsync<int>();
            return (assetMasterList, totalCount);            
        }
        public async Task<List<AssetMasterGeneralDTO>> GetByAssetNameAsync(string searchPattern)
        {
            const string query = @"            
            SELECT AM.Id,AM.CompanyId,AM.UnitId,AM.AssetCode,AM.AssetName,AM.AssetGroupId,AM.AssetCategoryId,AM.AssetSubCategoryId,AM.AssetParentId,AM.AssetType,AM.MachineCode,AM.Quantity
            ,AM.UOMId,AM.AssetDescription,AM.WorkingStatus,AM.AssetImage,AM.ISDepreciated,AM.IsTangible,AM.IsActive
            ,AM.CreatedBy,AM.CreatedDate,AM.CreatedByName,AM.CreatedIP,AM.ModifiedBy,AM.ModifiedDate,AM.ModifiedByName,AM.ModifiedIP
            ,AG.GroupName AssetGroupName,AC.CategoryName AssetCategoryDesc,A.Description AssetSubCategoryDesc,U.UOMName,MM.description WorkingStatusDesc,M.description AssetTypeDesc,isnull(AM1.AssetDescription,'') ParentAssetDesc
            FROM FixedAsset.AssetMaster AM
            INNER JOIN FixedAsset.AssetGroup AG on AG.Id=AM.AssetGroupId
            INNER JOIN FixedAsset.AssetCategories AC on AC.Id=AM.AssetCategoryId
            INNER JOIN FixedAsset.AssetSubCategories A on A.Id=AM.AssetSubCategoryId
            INNER JOIN FixedAsset.UOM U on U.Id=AM.UOMId
            INNER JOIN FixedAsset.MiscMaster MM on MM.Id =AM.WorkingStatus
            LEFT JOIN FixedAsset.MiscMaster M on M.Id =AM.AssetType
            LEFT JOIN FixedAsset.AssetMaster AM1 on AM1.Id =AM.AssetParentId
            WHERE (AM.AssetName LIKE @SearchPattern OR AM.AssetCode LIKE @SearchPattern) 
            AND  AM.IsDeleted=0 and AM.IsActive=1 ORDER BY AM.ID DESC";            
            var result = await _dbConnection.QueryAsync<AssetMasterGeneralDTO>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }

        public async Task<AssetMasterGeneralDTO> GetByIdAsync(int depGroupId)
        {
            const string query = @"            
            SELECT AM.Id,AM.CompanyId,AM.UnitId,AM.AssetCode,AM.AssetName,AM.AssetGroupId,AM.AssetCategoryId,AM.AssetSubCategoryId,AM.AssetParentId,AM.AssetType,AM.MachineCode,AM.Quantity
            ,AM.UOMId,AM.AssetDescription,AM.WorkingStatus,AM.AssetImage,AM.ISDepreciated,AM.IsTangible,AM.IsActive
            ,AM.CreatedBy,AM.CreatedDate,AM.CreatedByName,AM.CreatedIP,AM.ModifiedBy,AM.ModifiedDate,AM.ModifiedByName,AM.ModifiedIP
            ,AG.GroupName AssetGroupName,AC.CategoryName AssetCategoryDesc,A.Description AssetSubCategoryDesc,U.UOMName,MM.description WorkingStatusDesc,M.description AssetTypeDesc,isnull(AM1.AssetDescription,'') ParentAssetDesc
            FROM FixedAsset.AssetMaster AM
            INNER JOIN FixedAsset.AssetGroup AG on AG.Id=AM.AssetGroupId
            INNER JOIN FixedAsset.AssetCategories AC on AC.Id=AM.AssetCategoryId
            INNER JOIN FixedAsset.AssetSubCategories A on A.Id=AM.AssetSubCategoryId
            INNER JOIN FixedAsset.UOM U on U.Id=AM.UOMId
            INNER JOIN FixedAsset.MiscMaster MM on MM.Id =AM.WorkingStatus
            LEFT JOIN FixedAsset.MiscMaster M on M.Id =AM.AssetType
            LEFT JOIN FixedAsset.AssetMaster AM1 on AM1.Id =AM.AssetParentId
            WHERE AM.Id = @depGroupId AND AM.IsDeleted=0";
            var depreciationGroups = await _dbConnection.QueryFirstOrDefaultAsync<AssetMasterGeneralDTO>(query, new { depGroupId });           
            if (depreciationGroups is null)
            {
                throw new KeyNotFoundException($"DepreciationGroup with ID {depGroupId} not found.");
            }
            return depreciationGroups;
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWorkingStatusAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_WorkingStatus.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
        public async Task<List<Core.Domain.Entities.MiscMaster>> GetAssetTypeAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_AssetType.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }

        public async Task<bool> GetAssetChildDetails(int assetId)
        {
            const string query = @"
                    SELECT 1 FROM [FixedAsset].[AssetLocation] WHERE AssetId = @Id ;
                    SELECT 1 FROM [FixedAsset].[AssetPurchaseDetails] WHERE AssetId = @Id ;
                    SELECT 1 FROM [FixedAsset].[AssetWarranty] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetSpecifications] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetAmc] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetInsurance] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[AssetAdditionalCost] WHERE AssetId = @Id ;
                    SELECT 1 FROM [FixedAsset].[AssetDisposal] WHERE AssetId = @Id AND IsDeleted = 0;
                    SELECT 1 FROM [FixedAsset].[DepreciationDetail] WHERE AssetId = @Id ";
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = assetId });
                    
            var locationExists = await multi.ReadFirstOrDefaultAsync<int?>();  
            var purchaseExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var warrantyExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var specExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var amcExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var insuranceExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var additionalCostExists = await multi.ReadFirstOrDefaultAsync<int?>();
            var depreciationExists = await multi.ReadFirstOrDefaultAsync<int?>();
        
            return locationExists.HasValue || purchaseExists.HasValue || warrantyExists.HasValue  || specExists.HasValue  || amcExists.HasValue || insuranceExists.HasValue || additionalCostExists.HasValue || depreciationExists.HasValue ; 
        }

        public async Task<string?> GetLatestAssetCode(int companyId, int unitId, int assetGroupId, int assetCategoryId, int DepartmentId, int LocationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@UnitId", unitId);
            parameters.Add("@GroupId", assetGroupId);
            parameters.Add("@CategoryId", assetCategoryId);
            parameters.Add("@DeptId", DepartmentId);
            parameters.Add("@LocationId", LocationId);
            var newAssetCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.FAM_GetAssetCode", 
                parameters, 
                commandType: CommandType.StoredProcedure);
            return newAssetCode; 
        }

        public async Task<string> GetBaseDirectoryAsync()
        {
            var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(
                "dbo.FAM_GetBaseDirectory", 
                commandType: CommandType.StoredProcedure);
            return result ?? string.Empty; // return an empty string if result is null
        }

        public async  Task<List<Core.Domain.Entities.MiscMaster>>GetAssetPattern()
        {
            const string query = @"
            SELECT M.Id,MiscTypeId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP,MiscTypeCode
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
              var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_CodePattern.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();        
        }

        
    }
}