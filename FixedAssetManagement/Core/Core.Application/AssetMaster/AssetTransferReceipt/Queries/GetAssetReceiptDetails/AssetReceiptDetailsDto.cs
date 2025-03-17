using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptDetails
{
    public class AssetReceiptDetailsDto
    {

        public int AssetReceiptId {get;set;}
        public int AssetTransferId {get;set;}
        public DateTimeOffset DocDate { get; set; }
        public string? TransferType { get; set; } 
        public string? FromUnitname { get; set; }  
        public string? ToUnitname { get; set; } 
        public string? FromDepartment { get; set; }
        public string? ToDepartment { get; set; }  
        public int FromCustodianId  { get; set; } 
        public string? FromCustodianName  { get; set; } 
        public int ToCustodianId  { get; set; } 
        public string? ToCustodianName  { get; set; } 
   
    }
}