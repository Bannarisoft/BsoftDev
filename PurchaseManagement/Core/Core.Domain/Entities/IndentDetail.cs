using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class IndentDetail : BaseEntity
    {
        public int IndentHeaderId { get; set; }
        public int ItemId { get; set; }
        public decimal QuantityRequired { get; set; }
        public DateOnly RequiredDate { get; set; }
        public decimal TotalEstimatedCost { get; set; }
        public int PRConsumptionDays { get; set; }
        public string Remark { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public int StatusId { get; set; }
        public IndentHeader IndentHeader { get; set; }
        public MiscMaster Status { get; set; }
        
    }
}