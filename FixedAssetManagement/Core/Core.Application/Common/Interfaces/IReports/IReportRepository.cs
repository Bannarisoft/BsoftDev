
using Core.Application.Reports.AssetReport;

namespace Core.Application.Common.Interfaces.IReports
{
    public interface IReportRepository
    {
        Task<List<AssetReportDto>> AssetReportAsync( DateTimeOffset? fromDate,DateTimeOffset? toDate);
    }
}