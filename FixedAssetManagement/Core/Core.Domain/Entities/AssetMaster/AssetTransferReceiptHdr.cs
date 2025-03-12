using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Domain.Entities.AssetMaster
{
    public class AssetTransferReceiptHdr
    {
        public int Id {get;set;}
        public int AssetTransferId { get; set; }        
        public AssetTransferIssueHdr AssetTransferIssueHdr { get; set; } = null!; 
        public DateTimeOffset DocDate { get; set; }
        public int? TransferType { get; set; }        
        public MiscMaster TransferTypeReceiptMiscType { get; set; } = null!;  
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
        public ICollection<AssetTransferReceiptDtl>? AssetTransferReceiptDtl { get; set; }


    }
}