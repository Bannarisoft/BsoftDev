using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.StockLedger.Queries.GetStockLegerReport
{
    public class StockLedgerReportDto
    {
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? UOM { get; set; }

    public decimal OpeningQty { get; set; }
    public decimal OpeningValue { get; set; }

    public decimal ReceiptQty { get; set; }
    public decimal ReceiptValue { get; set; }

    public decimal IssueQty { get; set; }
    public decimal IssueValue { get; set; }

    public decimal ReturnQty { get; set; }
    public decimal ReturnValue { get; set; }

    public decimal ScrapReceiptQty { get; set; }
    public decimal ScrapReceiptValue { get; set; }

    public decimal ScrapIssueQty { get; set; }
    public decimal ScrapIssueValue { get; set; }
    public decimal ClosingQty { get; set; }
    public decimal ClosingValue { get; set; }
    }
}