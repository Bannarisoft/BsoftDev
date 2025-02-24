using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.UploadAssetWarranty
{
    public class UploadFileAssetWarrantyCommand : IRequest<ApiResponseDTO<AssetWarrantyDTO>>
    {
        public IFormFile? File { get; set; }
        public string? CompanyName { get; set; }  
        public string? UnitName { get; set; }  
        public string? AssetCode { get; set; }  
    }
}
