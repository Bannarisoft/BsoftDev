using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities
{
    public class ItemTransactions
    {
        public int Id { get; set; }
        public string? OldUnitCode { get; set; }
        public int TC { get; set; }
        public string? Type { get; set; } // "Issue" or "Return"
        public int DocNo { get; set; }
        public int DocSNo { get; set; }
        public DateTime DocDt { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public string? UOM { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        public string? CatDesc { get; set; }
        public string? GrpName { get; set; }
        public string? LifeType { get; set; }
        public int LifeSpan { get; set; }
        public string? DepName { get; set; }
        public DateTime CreatedDt { get; set; }
       
    }
}