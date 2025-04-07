using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending
{
    public class AssetTransferReceiptDtlDto
    {      
        public int AssetId { get; set; } 
        public int LocationId { get; set; }
        public int SubLocationId { get; set; } 
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public byte? AckStatus { get; set; } = 0;   // Default to 0
       // public DateTimeOffset? AckDate { get; set; }
        
    }
}