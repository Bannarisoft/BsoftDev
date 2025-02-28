using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetInsurance.Queries.GetAssetInsurance
{
    public class GetAssetInsuranceDto
    {
        public int Id { get; set; }
        public int  AssetId { get; set; }       
        public string? PolicyNo { get; set; }       
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string? PolicyAmount { get; set; }
        public string? VendorCode { get; set; }
        public string? RenewalStatus { get; set; }
        public DateTimeOffset RenewedDate { get; set; }
        public byte InsuranceStatus { get; set; }

        
    }
}