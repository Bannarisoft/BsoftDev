using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Reports.GetStockLegerReport
{
    public class GetStockLegerReportQuery : IRequest<ApiResponseDTO<List<StockLedgerReportDto>>>
    {
        public string? OldUnitcode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? ItemCode { get; set; }
        public int DepartmentId { get; set; }
       
    }
}