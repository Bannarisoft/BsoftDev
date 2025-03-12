using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTranferIssueApproval.Queries.GetAssetTransferIssueApproval
{
    public class AssetTransferIssueApprovalDto
    {
        public int Id {get;set;}
        public DateTimeOffset DocDate { get; set; }
        public string? TransferType { get; set; } 
        public string? FromUnitname { get; set; }  
        public string? ToUnitname { get; set; }  
        public string? FromDepartment { get; set; }
        public string? ToDepartment { get; set; }  
        public int FromCustodianId  { get; set; } 
        public int ToCustodianId  { get; set; } 
        public string? FromCustodianName  { get; set; } 
        public string? ToCustodianName  { get; set; } 
        public string? Status { get; set; } 
        
    }
}