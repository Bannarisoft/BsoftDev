
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.AssetReport
{
    public class AssetReportQuery : IRequest<ApiResponseDTO<List<AssetReportDto>>> 
    {
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
    }
}