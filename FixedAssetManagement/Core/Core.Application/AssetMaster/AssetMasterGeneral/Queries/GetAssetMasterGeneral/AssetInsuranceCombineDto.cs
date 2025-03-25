using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral
{
    public class AssetInsuranceCombineDto
    {
        public string? PolicyNo { get; set; }       
        public DateTimeOffset? StartDate { get; set; }        
        public DateTimeOffset? EndDate { get; set; }
        public string? PolicyAmount { get; set; }
        public string? VendorCode { get; set; }
    }
}