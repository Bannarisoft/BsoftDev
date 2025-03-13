using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
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
      public async Task<AssetTransferJsonDto> GetAssetTransferByIdAsync(int assetTransferId)
    {
        const string query = @"
            SELECT Id as AssetTransferId , DocDate, TransferType, FromUnitId, ToUnitId, FromDepartmentId, ToDepartmentId, 
                   FromCustodianId, ToCustodianId, Status, FromCustodianName, ToCustodianName
            FROM FixedAsset.AssetTransferIssueHdr
            WHERE Id = @AssetTransferId AND Status = 'Pending'
            FOR JSON PATH, INCLUDE_NULL_VALUES;

            SELECT AssetId, AssetValue 
            FROM FixedAsset.AssetTransferIssueDtl
            WHERE AssetTransferId = @AssetTransferId
            FOR JSON PATH, INCLUDE_NULL_VALUES;
        ";

        using var multiQuery = await _dbConnection.QueryMultipleAsync(query, new { assetTransferId });

        string headerJson = await multiQuery.ReadFirstOrDefaultAsync<string>();
        string detailsJson = await multiQuery.ReadFirstOrDefaultAsync<string>();

        if (string.IsNullOrWhiteSpace(headerJson))
        {
            return null;
        }

        var header = JsonSerializer.Deserialize<List<AssetTransferJsonDto>>(headerJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })?.FirstOrDefault();

        var details = JsonSerializer.Deserialize<List<AssetTransferDetailJsonDto>>(detailsJson ?? "[]", new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (header != null)
        {
            header.AssetTransferDetails = details ?? new List<AssetTransferDetailJsonDto>();
        }

        return header;
    }

    public async Task<List<GetAssetMasterDto>> GetAssetsByCategoryAsync(int assetCategoryId)
    {         
            const string query = @"SELECT Id as AssetId, AssetName FROM FixedAsset.AssetMaster WHERE AssetCategoryId = @assetCategoryId";                          
            var result = await _dbConnection.QueryAsync<GetAssetMasterDto>(query, new { assetCategoryId });         
            return result.ToList();      
    }
    }
}