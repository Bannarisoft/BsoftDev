using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetPurchaseCombineDto
    {          
        public string? BudgetType { get; set; }
        public string? OldUnitId { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorName { get; set; }
        public DateTimeOffset? PoDate { get; set; }
        public int  PoNo { get; set; }
        public int PoSno { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public int GrnNo { get; set; }
        public int GrnSno { get; set; }
        public DateTimeOffset? GrnDate { get; set; }
        public char? QcCompleted { get; set; }
        public decimal AcceptedQty { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal GrnValue { get; set; }
        public string? BillNo { get; set; }
        public DateTimeOffset? BillDate { get; set; }
        public string? Uom { get; set; }
        public string? BinLocation { get; set; }
        public string? PjYear { get; set; }
        public string? PjDocId { get; set; }
        public string? PjDocSr { get; set;}
        public int PjDocNo { get; set; }
        public int AssetSourceId { get; set; }   
        public DateTimeOffset? CapitalizationDate { get; set; }
    }
}