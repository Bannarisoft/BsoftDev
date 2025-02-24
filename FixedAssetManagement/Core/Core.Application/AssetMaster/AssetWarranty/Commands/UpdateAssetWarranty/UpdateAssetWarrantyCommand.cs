using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using MediatR;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.UpdateAssetWarranty
{
    public class UpdateAssetWarrantyCommand : IRequest<ApiResponseDTO<AssetWarrantyDTO>> 
    {
        public int Id { get; set; }      
        public int AssetId { get; set; }
        public DateTimeOffset? StartDate { get; set; } 
        public DateTimeOffset? EndDate { get; set; } 
        public int? Period { get; set; } 
        public int? WarrantyType { get; set; } 
        public string? Description { get; set; }    
        public string? ContactPerson { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }               
        //Service Center Details
        public int ServiceCountryId { get; set; }        
        public int ServiceStateId { get; set; }        
        public int ServiceCityId { get; set; }        
        public string? ServiceAddressLine1 { get; set; }        
        public string? ServiceAddressLine2 { get; set; }        
        public string? ServicePinCode { get; set; }        
        public string? ServiceContactPerson { get; set; }        
        public string? ServiceMobileNumber { get; set; }        
        public string? ServiceEmail { get; set; }   
        public string? ServiceClaimProcessDescription { get; set; } 
        public DateTimeOffset? ServiceLastClaimDate { get; set; } 
        public int? ServiceClaimStatus { get; set; } 
        public Status IsActive { get; set; }
    }
}