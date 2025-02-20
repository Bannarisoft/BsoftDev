
using System.Data;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Domain.Common;
using Core.Domain.Entities.AssetMaster;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetSpecification
{
    public class AssetSpecificationQueryRepository : IAssetSpecificationQueryRepository    
    {
          private readonly IDbConnection _dbConnection;
        public AssetSpecificationQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }     
        public async Task<(List<AssetSpecificationDTO>, int)> GetAllAssetSpecificationAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
             var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.AssetSpecifications A
                INNER JOIN FixedAsset.AssetMaster AM on AM.Id=A.AssetId
                INNER JOIN FixedAsset.SpecificationMaster SM on SM.Id=A.SpecificationId
                INNER JOIN FixedAsset.Manufacture MM on MM.Id=A.ManufactureId
                WHERE A.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (AS.AssetCode LIKE @Search OR AssetName LIKE @Search  OR SpecificationName LIKE @Search OR ManufactureName LIKE @Search )")}};

                SELECT A.Id,A.AssetId,A.ManufactureId,A.ManufactureDate,A.SpecificationId,A.SpecificationValue,A.SerialNumber,A.ModelNumber,A.IsActive
                ,A.CreatedBy,A.CreatedDate,A.CreatedByName,A.CreatedIP,A.ModifiedBy,A.ModifiedDate,A.ModifiedByName,A.ModifiedIP
                ,AM.AssetCode,AM.AssetName ,SM.SpecificationName,MM.ManufactureName
                FROM FixedAsset.AssetSpecifications A
                INNER JOIN FixedAsset.AssetMaster AM on AM.Id=A.AssetId
                INNER JOIN FixedAsset.SpecificationMaster SM on SM.Id=A.SpecificationId
                INNER JOIN FixedAsset.Manufacture MM on MM.Id=A.ManufactureId
                WHERE A.IsDeleted = 0
                {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (AssetCode LIKE @Search OR AssetName LIKE @Search  OR SpecificationName LIKE @Search OR ManufactureName LIKE @Search )")}}
                ORDER BY A.Id desc
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                SELECT @TotalCount AS TotalCount;
                """;
            var parameters = new
                       {
                           Search = $"%{SearchTerm}%",
                           Offset = (PageNumber - 1) * PageSize,
                           PageSize
                       };

            var assetSpecifications = await _dbConnection.QueryMultipleAsync(query, parameters);
            var assetSpecificationsList = (await assetSpecifications.ReadAsync<AssetSpecificationDTO>()).ToList();
            int totalCount = (await assetSpecifications.ReadFirstAsync<int>());             
            return (assetSpecificationsList, totalCount);             
        }

        public async Task<List<AssetSpecificationDTO>> GetByAssetSpecificationNameAsync(string searchPattern)
        {
            const string query = @"
            SELECT  A.Id,A.AssetId,A.ManufactureId,A.ManufactureDate,A.SpecificationId,A.SpecificationValue,A.SerialNumber,A.ModelNumber,A.IsActive
            ,A.CreatedBy,A.CreatedDate,A.CreatedByName,A.CreatedIP,A.ModifiedBy,A.ModifiedDate,A.ModifiedByName,A.ModifiedIP
            ,AM.AssetCode,AM.AssetName ,SM.SpecificationName,MM.ManufactureName
            FROM FixedAsset.AssetSpecifications A
            INNER JOIN FixedAsset.AssetMaster AM on AM.Id=A.AssetId
            INNER JOIN FixedAsset.SpecificationMaster SM on SM.Id=A.SpecificationId
            INNER JOIN FixedAsset.Manufacture MM on MM.Id=A.ManufactureId
            WHERE  (AssetCode LIKE @searchPattern OR AssetName LIKE @searchPattern  OR SpecificationName LIKE @searchPattern OR ManufactureName LIKE @searchPattern )
            AND  A.IsDeleted=0 and A.IsActive=1
            ORDER BY A.ID DESC";            
            var result = await _dbConnection.QueryAsync<AssetSpecificationDTO>(query, new { SearchPattern = $"%{searchPattern}%" });
            return result.ToList();
        }

        public async Task<AssetSpecificationDTO> GetByIdAsync(int assetId)
        {
            const string query = @"
            SELECT  A.Id,A.AssetId,A.ManufactureId,A.ManufactureDate,A.SpecificationId,A.SpecificationValue,A.SerialNumber,A.ModelNumber,A.IsActive
            ,A.CreatedBy,A.CreatedDate,A.CreatedByName,A.CreatedIP,A.ModifiedBy,A.ModifiedDate,A.ModifiedByName,A.ModifiedIP
            ,AM.AssetCode,AM.AssetName ,SM.SpecificationName,MM.ManufactureName
            FROM FixedAsset.AssetSpecifications A
            INNER JOIN FixedAsset.AssetMaster AM on AM.Id=A.AssetId
            INNER JOIN FixedAsset.SpecificationMaster SM on SM.Id=A.SpecificationId
            INNER JOIN FixedAsset.Manufacture MM on MM.Id=A.ManufactureId
            WHERE A.Id = @assetId AND A.IsDeleted=0";
            var assetSpecifications = await _dbConnection.QueryFirstOrDefaultAsync<AssetSpecificationDTO>(query, new { assetId });           
            if (assetSpecifications is null)
            {
                throw new KeyNotFoundException($"AssetSpecifications with ID {assetId} not found.");
            }
            return assetSpecifications;
        }
    }
}