
using System.Data;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Domain.Entities;
using Dapper;

namespace FAM.Infrastructure.Repositories.SpecificationMaster
{
    public class SpecificationMasterQueryRepository : ISpecificationMasterQueryRepository
    {
  private readonly IDbConnection _dbConnection;
        public SpecificationMasterQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<SpecificationMasters>, int)> GetAllSpecificationGroupAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.SpecificationMaster 
                WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SpecificationName LIKE @Search)")}};

                SELECT Id,SpecificationName,AssetGroupId,ISDefault,  IsActive
                ,CreatedBy,CreatedDate as CreatedAt,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
                FROM FixedAsset.SpecificationMaster  WHERE IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (SpecificationName LIKE @Search )")}}
                ORDER BY Id DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                SELECT @TotalCount AS TotalCount;
                """;
            var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

            var specificationMaster = await _dbConnection.QueryMultipleAsync(query, parameters);
            var specificationMasterList = (await specificationMaster.ReadAsync<SpecificationMasters>()).ToList();
            int totalCount = (await specificationMaster.ReadFirstAsync<int>());             
            return (specificationMasterList, totalCount);             
        }

        public async Task<List<SpecificationMasters>> GetBySpecificationNameAsync(int? AssetGroupId, string searchPattern)
        {
            const string query = @"
            SELECT Id,SpecificationName,AssetGroupId,ISDefault,  IsActive
            ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.SpecificationMaster
            WHERE (SpecificationName LIKE @SearchPattern) 
            AND  IsDeleted=0 and IsActive=1 and AssetGroupId=@AssetGroupId
            ORDER BY ID DESC";            
            var result = await _dbConnection.QueryAsync<SpecificationMasters>(query, 
                    new { SearchPattern = $"%{searchPattern}%", AssetGroupId });
            return result.ToList();
        }

        public async Task<SpecificationMasters> GetByIdAsync(int specId)
        {
            const string query = @"
            SELECT Id,SpecificationName,AssetGroupId,ISDefault,  IsActive
            ,CreatedBy,CreatedDate,CreatedByName,CreatedIP,ModifiedBy,ModifiedDate,ModifiedByName,ModifiedIP
            FROM FixedAsset.SpecificationMaster WHERE Id = @specId AND IsDeleted=0";
            var specificationMaster = await _dbConnection.QueryFirstOrDefaultAsync<SpecificationMasters>(query, new { specId });           
            if (specificationMaster is null)
            {
                throw new KeyNotFoundException($"Specification with ID {specId} not found.");
            }
            return specificationMaster;
        }
        public async Task<bool> SoftDeleteValidation(int Id)
        {
            const string query = @"
                SELECT 1 
                FROM FixedAsset.SpecificationMaster SM
                inner join  FixedAsset.AssetSpecifications AP on AP.SpecificationId = SM.Id
                WHERE SM.Id = @Id AND   AP.IsDeleted = 0;";        
            using var multi = await _dbConnection.QueryMultipleAsync(query, new { Id = Id });        
            var SpecificationExists = await multi.ReadFirstOrDefaultAsync<int?>();          
            return SpecificationExists.HasValue ;
        }
    }
}