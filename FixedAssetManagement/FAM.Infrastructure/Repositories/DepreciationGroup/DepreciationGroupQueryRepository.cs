using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IDepreciationGroup;
using Core.Domain.Entities;
using Dapper;

namespace FAM.Infrastructure.Repositories.DepreciationGroup
{
    public class DepreciationGroupQueryRepository : IDepreciationGroupQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public DepreciationGroupQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
        public async Task<(List<DepreciationGroups>, int)> GetAllDepreciationGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.DepreciationGroup 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR DepreciationGroupName LIKE @Search)")}};

                SELECT Id,Code,BookType,DepreciationGroupName,AssetGroupId,UsefulLife,DepreciationMethod,ResidualValue,SortOrder,  IsActive
                ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
                FROM FixedAsset.DepreciationGroup  WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (Code LIKE @Search OR DepreciationGroupName LIKE @Search )")}}
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

            var depreciationGroups = await _dbConnection.QueryMultipleAsync(query, parameters);
            var depreciationGroupList = (await depreciationGroups.ReadAsync<DepreciationGroups>()).ToList();
            int totalCount = (await depreciationGroups.ReadFirstAsync<int>());             
            return (depreciationGroupList, totalCount);             
        }

        public async Task<List<DepreciationGroups>> GetByDepreciationNameAsync(string searchPattern)
        {
            const string query = @"
            SELECT Id,Code,BookType,DepreciationGroupName,AssetGroupId,UsefulLife,DepreciationMethod,ResidualValue,SortOrder,  IsActive
            ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.DepreciationGroup 
            WHERE (DepreciationGroupName LIKE @SearchPattern OR Code LIKE @SearchPattern) 
            AND  IsDeleted=0 and IsActive=1
            ORDER BY ID DESC";            
            var result = await _dbConnection.QueryAsync<DepreciationGroups>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }

        public async Task<DepreciationGroups> GetByIdAsync(int depGroupId)
        {
            const string query = @"
            SELECT Id,Code,BookType,DepreciationGroupName,AssetGroupId,UsefulLife,DepreciationMethod,ResidualValue,SortOrder,  IsActive
            ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.DepreciationGroup WHERE Id = @depGroupId AND IsDeleted=0";
            var depreciationGroups = await _dbConnection.QueryFirstOrDefaultAsync<DepreciationGroups>(query, new { depGroupId });           
            if (depreciationGroups is null)
            {
                throw new KeyNotFoundException($"DepreciationGroup with ID {depGroupId} not found.");
            }
            return depreciationGroups;
        }
    }
}