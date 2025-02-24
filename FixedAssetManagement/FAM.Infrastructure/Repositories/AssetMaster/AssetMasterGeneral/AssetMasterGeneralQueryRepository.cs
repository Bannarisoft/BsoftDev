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
             var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.AssetMaster 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (AssetCode LIKE @Search OR AssetName LIKE @Search)")}};

   
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
                WHERE AM.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (AssetCode LIKE @Search OR AssetName LIKE @Search )")}}
                ORDER BY AM.Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                SELECT @TotalCount AS TotalCount;
                """;
            var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

            var assetMaster = await _dbConnection.QueryMultipleAsync(query, parameters);
            var assetMasterList = (await assetMaster.ReadAsync<AssetMasterGeneralDTO>()).ToList();
            int totalCount = (await assetMaster.ReadFirstAsync<int>());             
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
    }
}