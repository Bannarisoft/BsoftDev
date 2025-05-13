using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.StockLedger.Queries.GetCurrentStock
{
    public class CurrentStockDto 
    {
        public string? OldUnitId { get; set; }

        public string? ItemCode { get; set; }

        public string? ItemName { get; set; }
        public string? Uom { get; set; }
        public decimal StockQty { get; set; }
        public decimal StockValue { get; set; }
        public decimal Rate { get; set; }

    }
}