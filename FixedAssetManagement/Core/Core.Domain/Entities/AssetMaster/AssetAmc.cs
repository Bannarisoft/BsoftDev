using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Common;

namespace Core.Domain.Entities.AssetMaster
{
    public class AssetAmc : BaseEntity
    {
        public int AssetId { get; set; } 
        public AssetMasterGenerals AssetMasterAmcId { get; set; } = null!;
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; } 
        public int? Period { get; set; }   
        public string? VendorCode { get; set; }  
        public string? VendorName { get; set; }  
        public string? VendorPhone { get; set; }  
        public string? VendorEmail { get; set; }  
        public int? CoverageType { get; set; }        
        public MiscMaster CoverageMiscType { get; set; } = null!;         
        public int? FreeServiceCount  { get; set; }  
        public int? RenewalStatus { get; set; }        
        public MiscMaster RenewalStatusMiscType { get; set; } = null!;  
        public DateTimeOffset? RenewedDate { get; set; } 
       
    }
}