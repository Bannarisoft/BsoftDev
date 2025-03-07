
using System.Data;
using System.Text.Json;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
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
        //public async Task<(List<AssetSpecificationDTO>, int)> GetAllAssetSpecificationAsync(int PageNumber, int PageSize, string? SearchTerm)
                    
        public async Task<(List<AssetSpecificationJsonDto>, int)> GetAllAssetSpecificationAsync(int PageNumber, int PageSize, string? SearchTerm)
        {
            var parameters = new
            {
                PageNumber,
                PageSize,
                SearchTerm = string.IsNullOrEmpty(SearchTerm) ? null : SearchTerm
            };

            using var multi = await _dbConnection.QueryMultipleAsync("FAM_GetAssetSpecifications", parameters, commandType: CommandType.StoredProcedure);

            // Read JSON result
            string jsonResult = await multi.ReadFirstOrDefaultAsync<string>(); // JSON Data
            int totalCount = await multi.ReadFirstAsync<int>(); // Total Count

            // Ensure JSON is properly deserialized with nested specifications
            var assetSpecificationsList = string.IsNullOrWhiteSpace(jsonResult)
                ? new List<AssetSpecificationJsonDto>()
                : JsonSerializer.Deserialize<List<AssetSpecificationJsonDto>>(jsonResult, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return (assetSpecificationsList ?? new List<AssetSpecificationJsonDto>(), totalCount);
        }

        public async Task<List<AssetSpecificationJsonDto>> GetByAssetSpecificationNameAsync(string searchPattern)
        {
            const string query = @"
                SELECT (SELECT AM.Id AS AssetId,AM.AssetCode,AM.AssetName,MM.ManufactureName,(
                SELECT A.Id AS SpecificationId,A.SpecificationValue,A.SerialNumber,A.ModelNumber,SM.SpecificationName 
                FROM FixedAsset.AssetSpecifications A
                INNER JOIN FixedAsset.SpecificationMaster SM ON SM.Id = A.SpecificationId
                WHERE A.AssetId = AM.Id AND A.IsDeleted = 0 
                 FOR JSON PATH ) AS Specifications
                FROM FixedAsset.AssetMaster AM
                INNER JOIN FixedAsset.AssetSpecifications A ON A.AssetId = AM.Id
                INNER JOIN FixedAsset.Manufacture MM ON MM.Id = A.ManufactureId
                INNER JOIN FixedAsset.SpecificationMaster SM ON SM.Id = A.SpecificationId
                WHERE (AM.AssetCode LIKE '%' + @searchPattern + '%'                 
                OR SM.SpecificationName LIKE '%' + @searchPattern + '%'   )
                AND A.IsDeleted = 0  AND A.IsActive = 1
                GROUP BY AM.Id, AM.AssetCode, AM.AssetName, MM.ManufactureName
                FOR JSON PATH, INCLUDE_NULL_VALUES
                ) AS JsonResult;";            
           // ✅ Fetch JSON as string
            string jsonResult = await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { SearchPattern = $"%{searchPattern}%" });

            if (string.IsNullOrWhiteSpace(jsonResult))
            {
                return new List<AssetSpecificationJsonDto>();
            }
            // ✅ Deserialize JSON manually
            var assetSpecificationsList = JsonSerializer.Deserialize<List<AssetSpecificationJsonDto>>(jsonResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return assetSpecificationsList ?? new List<AssetSpecificationJsonDto>();
        }
        public async Task<AssetSpecificationJsonDto> GetByIdAsync(int assetId)
        {
            const string query = @"
                SELECT AM.Id AS AssetId,AM.AssetCode,AM.AssetName,MM.ManufactureName,(
                SELECT A.Id AS SpecificationId,A.SpecificationValue,A.SerialNumber,A.ModelNumber,SM.SpecificationName
                FROM FixedAsset.AssetSpecifications A
                INNER JOIN FixedAsset.SpecificationMaster SM ON SM.Id = A.SpecificationId
                WHERE A.AssetId = AM.Id AND A.IsDeleted = 0 FOR JSON PATH ) AS Specifications
                FROM FixedAsset.AssetMaster AM
                left JOIN FixedAsset.AssetSpecifications A ON A.AssetId = AM.Id
                INNER JOIN FixedAsset.Manufacture MM ON MM.Id = A.ManufactureId
                WHERE A.Id =@assetId AND A.IsDeleted = 0 
                GROUP BY AM.Id,A.Id, AM.AssetCode, AM.AssetName, MM.ManufactureName
                FOR JSON PATH, INCLUDE_NULL_VALUES;";
             // ✅ Fetch JSON as string
            string jsonResult = await _dbConnection.QueryFirstOrDefaultAsync<string>(query, new { assetId });

            if (string.IsNullOrWhiteSpace(jsonResult))
            {
                throw new KeyNotFoundException($"AssetSpecifications with ID {assetId} not found.");
            }

            // ✅ Deserialize JSON manually
            var assetSpecification = JsonSerializer.Deserialize<List<AssetSpecificationJsonDto>>(jsonResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })?.FirstOrDefault();

            return assetSpecification ?? throw new KeyNotFoundException($"AssetSpecifications with ID {assetId} not found.");
        }       
    }
}