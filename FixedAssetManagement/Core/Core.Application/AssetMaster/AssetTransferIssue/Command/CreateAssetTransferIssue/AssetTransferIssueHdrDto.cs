using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueById;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;

namespace Core.Application.AssetMaster.AssetTransferIssue.Queries.GetAssetTransfered
{
    public class AssetTransferIssueHdrDto
    {
    
    
    public DateTimeOffset DocDate { get; set; }
    public int TransferType { get; set; }
    public int FromUnitId { get; set; }
    public int ToUnitId { get; set; }
    public int FromDepartmentId { get; set; }
    public int ToDepartmentId { get; set; }
    public int FromCustodianId { get; set; }

    public string? FromCustodianName { get; set; }
    public int ToCustodianId { get; set; }
    public string? ToCustodianName { get; set; }
    public string? GatePassNo { get; set; }
 
    // List of assets associated with this transfer
        public List<AssetTransferIssueDtlDto>? AssetTransferIssueDtls { get; set; }


    }
}