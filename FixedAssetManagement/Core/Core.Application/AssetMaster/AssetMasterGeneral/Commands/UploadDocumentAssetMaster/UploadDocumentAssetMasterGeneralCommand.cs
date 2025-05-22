using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UploadDocumentAssetMaster
{
    public class UploadDocumentAssetMasterGeneralCommand : IRequest<ApiResponseDTO<AssetMasterDocumentDto>>
    {
        public IFormFile? File { get; set; }    
    }
}
