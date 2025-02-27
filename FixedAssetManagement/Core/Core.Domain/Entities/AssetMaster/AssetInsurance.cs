using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Domain.Entities.AssetMaster
{
    public class AssetInsurance 
    {
        public int Id { get; set; }
        public int  AssetId { get; set; }
         public AssetMasterGenerals? AssetMaster  { get; set; }         
        public string? PolicyNo { get; set; }       
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string? PolicyAmount { get; set; }
        public string? VendorCode { get; set; }
        public string? RenewalStatus { get; set; }
        public DateTimeOffset RenewedDate { get; set; }
        public string? InsuranceStatus { get; set; }
        
    
    }
}