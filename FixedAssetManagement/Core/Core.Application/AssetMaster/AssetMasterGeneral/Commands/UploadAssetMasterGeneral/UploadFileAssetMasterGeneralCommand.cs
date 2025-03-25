using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadAssetMasterGeneral
{
    public class UploadFileAssetMasterGeneralCommand : IRequest<ApiResponseDTO<AssetMasterImageDto>>
    {
        public IFormFile? File { get; set; }
        public string? CompanyName { get; set; }  
        public string? UnitName { get; set; }  
      /*   public string? AssetCode { get; set; }   */
    }
}
