
using Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered;
using Core.Application.Reports.AssetReport;
using Core.Application.Reports.AssetTransferReport;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {
        Task<List<AssetReportDto>> AssetReportAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate);
        Task<List<AssetTransferDetailsDto>> AssetTransferReportAsync( DateTimeOffset? fromDate,DateTimeOffset? toDate);
    }
}