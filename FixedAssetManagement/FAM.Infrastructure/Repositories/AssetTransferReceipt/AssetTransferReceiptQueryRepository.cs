using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Application.Common.Interfaces.IAssetTransferReceipt;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetTransferReceipt
{
    public class AssetTransferReceiptQueryRepository : IAssetTransferReceiptQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public AssetTransferReceiptQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<(List<AssetTransferReceiptPendingDto>, int)> GetAllPendingAssetTransferAsync(int PageNumber, int PageSize, string? TransferType, DateTimeOffset? FromDate, DateTimeOffset? ToDate)
        {
            var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.AssetTransferIssueHdr a
                INNER JOIN FixedAsset.AssetTransferIssueDtl b on a.Id=b.AssetTransferId
                INNER JOIN FixedAsset.AssetMaster m on b.AssetId=m.Id
                INNER JOIN FixedAsset.MiscMaster c on  a.TransferType= c.Id
                INNER JOIN [Bannari].AppData.Unit d on a.FromUnitId= d.Id
                INNER JOIN [Bannari].AppData.Unit e on a.ToUnitId= e.Id
                INNER JOIN [Bannari].AppData.Department f on a.FromDepartmentId= f.Id
                INNER JOIN [Bannari].AppData.Department g on a.FromDepartmentId= g.Id
                WHERE a.AckStatus= 0 and  a.Status = 'Approved'
                {{(string.IsNullOrEmpty(TransferType) ? "" : "AND a.Id LIKE @Search")}}
                {{(FromDate.HasValue ? "AND a.DocDate >= @FromDate" : "")}}
                {{(ToDate.HasValue ? "AND a.DocDate <= @ToDate" : "")}};

                SELECT 
                B.AssetTransferId,
                A.DocDate,
                C.Description AS TransferType,
                M.AssetCode,
                M.AssetName,
                A.FromUnitId,
                D.UnitName AS FromUnitName,
                A.ToUnitId,
                E.UnitName AS ToUnitName,
                A.FromDepartmentId,
                F.DeptName AS FromDepartment,
                A.ToDepartmentId,
                G.DeptName AS ToDepartment,
                A.FromCustodianId, 
                A.FromCustodianName,
                A.ToCustodianId,
                A.ToCustodianName,
                A.Status 
                FROM FixedAsset.AssetTransferIssueHdr A 
                INNER JOIN FixedAsset.AssetTransferIssueDtl B ON A.Id = B.AssetTransferId
                INNER JOIN FixedAsset.AssetMaster M ON B.AssetId = M.Id
                INNER JOIN FixedAsset.MiscMaster C ON A.TransferType = C.Id
                INNER JOIN [Bannari].AppData.Unit D ON A.FromUnitId = D.Id
                INNER JOIN [Bannari].AppData.Unit E ON A.ToUnitId = E.Id
                INNER JOIN [Bannari].AppData.Department F ON A.FromDepartmentId = F.Id
                INNER JOIN [Bannari].AppData.Department G ON A.ToDepartmentId = G.Id
                WHERE A.AckStatus= 0 and  A.Status = 'Approved'
                {{(string.IsNullOrEmpty(TransferType) ? "" : "AND a.Id LIKE @Search")}}
                {{(FromDate.HasValue ? "AND a.DocDate >= @FromDate" : "")}}
                {{(ToDate.HasValue ? "AND a.DocDate <= @ToDate" : "")}}
                ORDER BY B.AssetTransferId ASC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT @TotalCount AS TotalCount;
            """;

            var parameters = new
            {
                Search = $"%{TransferType}%",
                FromDate,
                ToDate,
                Offset = (PageNumber - 1) * PageSize,
                PageSize
            };

            var assetTransferIssue = await _dbConnection.QueryMultipleAsync(query, parameters);
            var assetTransferIssueList = (await assetTransferIssue.ReadAsync<AssetTransferReceiptPendingDto>()).ToList();
            int totalCount = await assetTransferIssue.ReadFirstAsync<int>();

            return (assetTransferIssueList, totalCount);
        }
    }
}