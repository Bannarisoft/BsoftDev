using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetInsurance.Commands.UpdateAssetInsurance
{
    public class UpdateAssetInsuranceCommand  :   IRequest<ApiResponseDTO<bool>>  
    {
         public int  AssetId { get; set; }       
        public string? PolicyNo { get; set; }       
        public DateTimeOffset StartDate { get; set; }
        public int Insuranceperiod { get; set; }  
        public DateTimeOffset EndDate { get; set; }
        public string? PolicyAmount { get; set; }
        public string? VendorCode { get; set; }
        public string? RenewalStatus { get; set; }
        public DateTimeOffset RenewedDate { get; set; }
        public byte InsuranceStatus { get; set; }
    }
}