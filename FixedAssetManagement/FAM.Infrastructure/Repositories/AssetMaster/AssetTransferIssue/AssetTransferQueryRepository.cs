using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetMaster.AssetTransferIssue
{
    public class AssetTransferQueryRepository : IAssetTransferQueryRepository
    {
        private readonly IDbConnection _dbConnection;

         public AssetTransferQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        

         public async Task<(List<AssetTransferDto>, int)> GetAllAsync(int PageNumber, int PageSize, string? SearchTerm)        
        {
          var query = $$"""
        DECLARE @TotalCount INT;
        SELECT @TotalCount = COUNT(*) 
        FROM FixedAsset.AssetMaster A
        INNER JOIN FixedAsset.AssetLocation B ON A.Id = B.AssetId
        WHERE  A.IsDeleted = 0
        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (A.AssetName LIKE @Search OR B.CustodianId LIKE @Search)")}};

        SELECT A.Id as Assetid , A.AssetCategoryId, A.AssetName, A.UnitId, 
               B.DepartmentId, B.CustodianId, B.UserID, B.UnitId, 
               B.LocationId, B.SubLocationId, A.IsDeleted
        FROM FixedAsset.AssetMaster A
        INNER JOIN FixedAsset.AssetLocation B ON A.Id = B.AssetId
        WHERE  A.IsDeleted = 0
        {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (A.AssetName LIKE @Search OR B.CustodianId LIKE @Search)")}}
        ORDER BY A.Id DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT @TotalCount AS TotalCount;
    """;

    var parameters = new
    {
        Search = $"%{SearchTerm}%",
        Offset = (PageNumber - 1) * PageSize,
        PageSize
       
        
    };

    var assetTransfer = await _dbConnection.QueryMultipleAsync(query, parameters);
    var assetTransferList = (await assetTransfer.ReadAsync<AssetTransferDto>()).ToList();
    int totalCount = await assetTransfer.ReadFirstAsync<int>();

    return (assetTransferList, totalCount);
   }

        // public async Task<AssetTransferDto> GetByIdAsync(int assetId)
        // {
        //     const string query = @"
        //    select a.id,a.AssetCategoryId,a.AssetName ,  a.UnitId,b.DepartmentId,b.CustodianId,b.UserID,b.UnitId,b.LocationId,b.SubLocationId,, a.IsDeleted
        //     from  FixedAsset.AssetMaster  a 
        //     inner join  FixedAsset.AssetLocation b 
        //     on a.Id=b.AssetId where a.id = @assetId AND A.IsDeleted=0";
        //     var assetSpecifications = await _dbConnection.QueryFirstOrDefaultAsync<AssetTransferDto>(query, new { assetId });           
        //     if (assetSpecifications is null)
        //     {
        //         throw new KeyNotFoundException($"AssetSpecifications with ID {assetId} not found.");
        //     }
        //     return assetSpecifications;
        // }
    }
}