using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending
{
    public class AssetTransferReceiptPendingDto
    {
     
        public int AssetTransferId {get;set;}
        public DateTimeOffset DocDate { get; set; }
        public string? TransferType { get; set; } 
        // public string? AssetCode { get; set; }
        // public string? AssetName { get; set; }
        // public int AssetId { get; set; }       
        public string? FromUnitname { get; set; }  
        public string? ToUnitname { get; set; } 
        public string? FromDepartment { get; set; }
        public string? ToDepartment { get; set; }  
        public string? FromCustodianName  { get; set; } 
        public string? ToCustodianName  { get; set; } 
        public string? Status { get; set; }
        public string? Sdcno { get; set; }
        public string? GatePassNo { get; set; }
        

    }
}
