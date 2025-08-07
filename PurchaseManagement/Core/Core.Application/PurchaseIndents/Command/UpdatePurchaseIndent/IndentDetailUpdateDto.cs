using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent
{
    public class IndentDetailUpdateDto
    {
        public int IndentHeaderId { get; set; }
        public int ItemId { get; set; }
        public decimal QuantityRequired { get; set; }
        public DateOnly RequiredDate { get; set; }
        public decimal TotalEstimatedCost { get; set; }
        public int PRConsumptionDays { get; set; }
        public string Remark { get; set; }
    }
}