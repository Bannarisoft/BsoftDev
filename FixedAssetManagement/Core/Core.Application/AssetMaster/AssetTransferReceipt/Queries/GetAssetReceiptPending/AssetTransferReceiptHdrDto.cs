using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending
{
    public class AssetTransferReceiptHdrDto
    {
        public int AssetTransferId { get; set; }        
        public DateTimeOffset DocDate { get; set; }
        public int? TransferType { get; set; }         
        public int FromUnitId { get; set; }  
        public int ToUnitId { get; set; } 
        public int FromDepartmentId  { get; set; } 
        public int ToDepartmentId  { get; set; } 
        public int FromCustodianId  { get; set; } 
        public string? FromCustodianName { get; set; }
        public int ToCustodianId  { get; set; } 
        public string? ToCustodianName { get; set; }
        public string? Sdcno { get; set; }
        public string? GatePassNo  { get; set; }
        public int? AuthorizedBy { get; set; }
        public DateTimeOffset? AuthorizedDate { get; set; }
        public string? AuthorizedByName { get; set; }
        public string? AuthorizedIP { get; set; }
        public string? Remarks { get; set; }
        public List<AssetTransferReceiptDtlDto>? AssetTransferReceiptDtl { get; set; }
    }
}