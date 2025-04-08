using System.Data;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval;
using Core.Application.Common.Interfaces.IAssetTransferIssueApproval;
using Dapper;

namespace FAM.Infrastructure.Repositories.AssetTransferIssueApproval
{
    public class AssetTransferIssueQueryRepository : IAssetTransferIssueApprovalQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public AssetTransferIssueQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

    public async Task<(List<AssetTransferIssueApprovalDto>, int)> GetAllPendingAssetTransferAsync(
    int PageNumber, 
    int PageSize, 
    string? TransferType, 
    DateTimeOffset? FromDate, 
    DateTimeOffset? ToDate)
        {
            var query = $$"""
                DECLARE @TotalCount INT;
                SELECT @TotalCount = COUNT(*) 
                FROM FixedAsset.AssetTransferIssueHdr a
                INNER JOIN FixedAsset.MiscMaster c ON a.TransferType = c.Id
                INNER JOIN [Bannari].AppData.Unit d ON a.FromUnitId = d.Id
                INNER JOIN [Bannari].AppData.Unit e ON a.ToUnitId = e.Id
                INNER JOIN [Bannari].AppData.Department f ON a.FromDepartmentId = f.Id
                INNER JOIN [Bannari].AppData.Department g ON a.ToDepartmentId = g.Id
                WHERE a.Status = 'Pending'
                {{(string.IsNullOrEmpty(TransferType) ? "" : "AND a.TransferType LIKE @Search")}}
                {{(FromDate.HasValue ? "AND CAST(a.DocDate AS DATE) >= CAST(@FromDate AS DATE)" : "")}}
                {{(ToDate.HasValue ? "AND CAST(a.DocDate AS DATE) <= CAST(@ToDate AS DATE)" : "")}};

                SELECT 
                    a.Id, 
                    a.DocDate, 
                    c.Description AS TransferType, 
                    d.UnitName AS FromUnitName, 
                    e.UnitName AS ToUnitName, 
                    f.DeptName AS FromDepartment, 
                    g.DeptName AS ToDepartment, 
                    a.FromCustodianId, 
                    a.FromCustodianName, 
                    a.ToCustodianId,
                    a.ToCustodianName, 
                    a.Status
                    FROM FixedAsset.AssetTransferIssueHdr a
                INNER JOIN FixedAsset.MiscMaster c ON a.TransferType = c.Id
                INNER JOIN [Bannari].AppData.Unit d ON a.FromUnitId = d.Id
                INNER JOIN [Bannari].AppData.Unit e ON a.ToUnitId = e.Id
                INNER JOIN [Bannari].AppData.Department f ON a.FromDepartmentId = f.Id
                INNER JOIN [Bannari].AppData.Department g ON a.ToDepartmentId = g.Id
                WHERE a.Status = 'Pending'
                {{(string.IsNullOrEmpty(TransferType) ? "" : "AND a.TransferType LIKE @Search")}}
                {{(FromDate.HasValue ? "AND CAST(a.DocDate AS DATE) >= CAST(@FromDate AS DATE)" : "")}}
                {{(ToDate.HasValue ? "AND CAST(a.DocDate AS DATE) <= CAST(@ToDate AS DATE)" : "")}}
                ORDER BY a.Id DESC
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
            var assetTransferIssueList = (await assetTransferIssue.ReadAsync<AssetTransferIssueApprovalDto>()).ToList();
            int totalCount = await assetTransferIssue.ReadFirstAsync<int>();

            return (assetTransferIssueList, totalCount);
        }   

    public async Task<List<Core.Domain.Entities.AssetMaster.AssetTransferIssueApproval>> GetByAssetTransferIdAsync(int Id)
    {
            const string query = @"
            SELECT 
                a.Id,
                a.DocDate,
                b.AssetId,
                c.AssetCode,
                c.AssetName AS AssetName,
                b.AssetValue 
            FROM FixedAsset.AssetTransferIssueHdr a 
            INNER JOIN FixedAsset.AssetTransferIssueDtl b ON a.Id = b.AssetTransferId
            INNER JOIN FixedAsset.AssetMaster c ON b.AssetId = c.Id
            WHERE a.Status = 'Pending' AND a.Id = @Id";

        var assetTransfers = await _dbConnection.QueryAsync<Core.Domain.Entities.AssetMaster.AssetTransferIssueApproval>(query, new { Id });

        return assetTransfers.ToList(); // Ensure it returns a List
    }
    }
}