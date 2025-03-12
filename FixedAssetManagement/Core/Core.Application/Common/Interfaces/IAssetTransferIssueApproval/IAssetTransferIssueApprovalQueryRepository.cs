using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval;
using Core.Domain.Entities.AssetMaster;


namespace Core.Application.Common.Interfaces.IAssetTransferIssueApproval
{
    public interface IAssetTransferIssueApprovalQueryRepository
    { 
        Task<(List<AssetTransferIssueApprovalDto>, int)> GetAllPendingAssetTransferAsync(
        int PageNumber, 
        int PageSize, 
        string? SearchTerm, 
        DateTimeOffset? FromDate, 
        DateTimeOffset? ToDate);
        Task<List<AssetTransferIssueApproval>> GetByAssetTransferIdAsync(int Id);

    }
}