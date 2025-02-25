using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Domain.Entities
{
    public class Insurance 
    {
        public int  AssetId { get; set; }
         public AssetMasterGenerals AssetMasterId { get; set; } = null!;
        public string? PolicyNo { get; set; }       
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string? PolicyAmount { get; set; }
        public int VendorId { get; set; }
        public DateTimeOffset NextRenewalDate { get; set; }

        public string? RenewalStatus { get; set; }

        public DateTimeOffset RenewedDate { get; set; }
        public string? InsuranceStatus { get; set; }

       

        


        
    }
}