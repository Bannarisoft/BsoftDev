using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferIssue.Command.CreateAssetTransferIssue
{
    public class CreateAssetTransferIssueCommand  
    {
        
       public int AssetTransferId { get; set; }

       public DateTimeOffset DocDate { get; set; }
        public int? TransferType { get; set; }      
        public int FromUnitId { get; set; }
        public int ToUnitId { get; set; }
        public int FromDepartmentId  { get; set; }
        public int ToDepartmentId  { get; set; }
        public int FromCustodianId  { get; set; }
        public int ToCustodianId  { get; set; }
        public string? Status { get; set; }
       
   
        


    }
}