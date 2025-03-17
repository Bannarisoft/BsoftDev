using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAllAssetTransfer;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssertByCategory;
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetDtlToTransfer;
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
        


         public async Task<(List<AssetTransferDto>, int)> GetAllAsync(int PageNumber, int PageSize ,string? SearchTerm, DateTimeOffset? FromDate , DateTimeOffset? ToDate )        
        {

                var query = $$"""
            DECLARE @TotalCount INT;
            SELECT @TotalCount = COUNT(*) 
            FROM FixedAsset.AssetTransferIssueHdr
            WHERE 1 = 1 
             {{(FromDate.HasValue ? "AND DocDate >= @FromDate" : "")}}
            {{(ToDate.HasValue ? "AND DocDate <= @ToDate" : "")}}
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(Id AS NVARCHAR) LIKE @Search)") }};


            SELECT Id, DocDate, TransferType, FromUnitId, ToUnitId, 
                FromDepartmentId, ToDepartmentId, FromCustodianId, ToCustodianId, 
                Status, CreatedBy, CreatedDate, CreatedByName, CreatedIP, 
                ModifiedBy, ModifiedDate, ModifiedByName, ModifiedIP, 
                AuthorizedBy, AuthorizedDate, AuthorizedByName, AuthorizedIP, 
                FromCustodianName, ToCustodianName, AckStatus
            FROM FixedAsset.AssetTransferIssueHdr
            WHERE   1 = 1          
            {{(FromDate.HasValue ? "AND DocDate >= @FromDate" : "")}}
            {{(ToDate.HasValue ? "AND DocDate < @ToDate" : "")}}
            {{(string.IsNullOrEmpty(SearchTerm) ? "" : "AND (CAST(Id AS NVARCHAR) LIKE @Search)") }}        
            ORDER BY Id DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;


            SELECT @TotalCount AS TotalCount;
            """;


            var parameters = new
            {  
                FromDate,
                ToDate  = ToDate?.Date.AddDays(1),
                Search = $"%{SearchTerm}%",
                Offset = (PageNumber - 1) * PageSize,
                PageSize               
            };


            var assetTransfers = await _dbConnection.QueryMultipleAsync(query, parameters);
            var assetTransferList = (await assetTransfers.ReadAsync<AssetTransferDto>()).ToList();
            int totalCount = await assetTransfers.ReadFirstAsync<int>();


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
    
     public async Task<GetAssetDetailsToTransferHdrDto> GetAssetDetailsToTransferByIdAsync(int assetId)
    {
                    const string query = @"
                    -- Get Asset Master Details
                    SELECT 
                        A.Id AS AssetId, A.CreatedDate as DocDate,  H.CategoryName, A.AssetCode, A.AssetName, 
                        A.UnitId, G.UnitName, B.LocationId, C.LocationName, B.SubLocationId, D.SubLocationName, 
                        B.DepartmentId, F.DeptName AS DepartmentName
                    FROM FixedAsset.AssetMaster A
                    INNER JOIN FixedAsset.AssetLocation B ON A.ID = B.AssetId
                    INNER JOIN FixedAsset.Location C ON B.LocationId = C.Id
                    INNER JOIN FixedAsset.SubLocation D ON B.SubLocationId = D.Id
                    INNER JOIN Bannari.AppData.Department F ON B.DepartmentId = F.Id
                    INNER JOIN Bannari.AppData.Unit G ON A.UnitId = G.Id
                    INNER JOIN FixedAsset.AssetCategories H ON A.AssetCategoryId = H.Id
                    WHERE A.Id = @AssetId  
                    FOR JSON PATH, INCLUDE_NULL_VALUES;

                    -- Get Asset Transfer Issue Details
                    SELECT AssetId ,GrnValue as AssetValue
                    FROM FixedAsset.AssetPurchaseDetails 
                    WHERE AssetId = @AssetId
                    FOR JSON PATH, INCLUDE_NULL_VALUES;
                ";

                using var multiQuery = await _dbConnection.QueryMultipleAsync(query, new { AssetId = assetId });

                string assetJson = await multiQuery.ReadFirstOrDefaultAsync<string>();
                string transferJson = await multiQuery.ReadFirstOrDefaultAsync<string>();

                if (string.IsNullOrWhiteSpace(assetJson))
                {
                    return null; // Asset not found
                }

          
                var assetDetails = JsonSerializer.Deserialize<List<GetAssetDetailsToTransferHdrDto>>(assetJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })?.FirstOrDefault();

                // Deserialize Transfer Issue Details
                var transferDetails = JsonSerializer.Deserialize<List<GetAssetDetailsToTransferDto>>(transferJson ?? "[]", new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (assetDetails != null)
                {
                    assetDetails.GetAssetDetailToTransfer = transferDetails ?? new List<GetAssetDetailsToTransferDto>();
                }
                return assetDetails;                
                }
                public async Task<bool> IsAssetPendingOrApprovedAsync(int assetId)
                {
                    const string query = @"
                        SELECT 1 FROM FixedAsset.AssetTransferIssueHdr A
                        INNER JOIN FixedAsset.AssetTransferIssueDtl B ON A.Id = B.AssetTransferId
                        WHERE B.AssetId = @assetId 
                        AND (A.Status = 'Pending' OR (A.Status = 'Approved' AND A.AckStatus <> 1))";

                    var result = await _dbConnection.QueryFirstOrDefaultAsync<int?>(query, new { assetId });
                    return result.HasValue; // If record exists, return true (restricted)
                }

               public async Task<List<GetAllTransferDtlDto>> GetAssetTransferByIDAsync(int assetTransferId)
                {         
                        const string query = @"SELECT  A.Id,A.AssetTransferId,A.AssetId,B.AssetCode,B.AssetName,A.AssetValue  FROM FixedAsset.AssetTransferIssueDtl A 
			                                 INNER JOIN  FixedAsset.AssetMaster B on  A.AssetId=B.ID WHERE AssetTransferId = @assetTransferId";                          
                        var result = await _dbConnection.QueryAsync<GetAllTransferDtlDto>(query, new { assetTransferId });         
                        return result.ToList();      
                }   
    }

}