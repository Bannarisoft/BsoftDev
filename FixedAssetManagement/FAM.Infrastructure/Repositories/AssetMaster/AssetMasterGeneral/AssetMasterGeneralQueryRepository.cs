using System.Data;
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
        public async Task<(List<AssetMasterGenerals>, int)> GetAllAssetAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.AssetMaster 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (AssetCode LIKE @Search OR AssetName LIKE @Search)")}};

                SELECT Id,CompanyId,UnitId,AssetCode,AssetName,AssetGroupId,AssetCategoryId,AssetSubCategoryId,AssetParentId,AssetType,MachineCode,Quantity
                ,UOMId,AssetDescription,WorkingStatus,AssetImage,ISDepreciated,IsTangible,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
                FROM FixedAsset.AssetMaster  WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (AssetCode LIKE @Search OR AssetName LIKE @Search )")}}
                ORDER BY Id desc
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
            var assetMasterList = (await assetMaster.ReadAsync<AssetMasterGenerals>()).ToList();
            int totalCount = (await assetMaster.ReadFirstAsync<int>());             
            return (assetMasterList, totalCount);             
        }
        public async Task<List<AssetMasterGenerals>> GetByAssetNameAsync(string searchPattern)
        {
            const string query = @"
            SELECT Id,CompanyId,UnitId,AssetCode,AssetName,AssetGroupId,AssetCategoryId,AssetSubCategoryId,AssetParentId,AssetType,MachineCode,Quantity
                ,UOMId,AssetDescription,WorkingStatus,AssetImage,ISDepreciated,IsTangible,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.AssetMaster 
            WHERE (AssetName LIKE @SearchPattern OR AssetCode LIKE @SearchPattern) 
            AND  IsDeleted=0 and IsActive=1
            ORDER BY ID DESC";            
            var result = await _dbConnection.QueryAsync<AssetMasterGenerals>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }

        public async Task<AssetMasterGenerals> GetByIdAsync(int depGroupId)
        {
            const string query = @"
            SELECT Id,CompanyId,UnitId,AssetCode,AssetName,AssetGroupId,AssetCategoryId,AssetSubCategoryId,AssetParentId,AssetType,MachineCode,Quantity
                ,UOMId,AssetDescription,WorkingStatus,AssetImage,ISDepreciated,IsTangible,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.AssetMaster WHERE Id = @depGroupId AND IsDeleted=0";
            var depreciationGroups = await _dbConnection.QueryFirstOrDefaultAsync<AssetMasterGenerals>(query, new { depGroupId });           
            if (depreciationGroups is null)
            {
                throw new KeyNotFoundException($"DepreciationGroup with ID {depGroupId} not found.");
            }
            return depreciationGroups;
        }

        public async Task<List<Core.Domain.Entities.MiscMaster>> GetWorkingStatusAsync()
        {
            const string query = @"
            SELECT M.Id,MiscTypeMasterId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeMasterId
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
            SELECT M.Id,MiscTypeMasterId,Code,M.Description,SortOrder,  M.IsActive
            ,M.CreatedBy,M.CreatedDate,M.CreatedByName,M.CreatedIP,M.ModifiedBy,M.ModifiedDate,M.ModifiedByName,M.ModifiedIP
            FROM FixedAsset.MiscMaster M
            INNER JOIN FixedAsset.MiscTypeMaster T on T.ID=M.MiscTypeMasterId
            WHERE (MiscTypeCode = @MiscTypeCode) 
            AND  M.IsDeleted=0 and M.IsActive=1
            ORDER BY M.ID DESC";    
            var parameters = new { MiscTypeCode = MiscEnumEntity.Asset_AssetType.MiscCode };        
            var result = await _dbConnection.QueryAsync<Core.Domain.Entities.MiscMaster>(query,parameters);
            return result.ToList();
        }
    }
}