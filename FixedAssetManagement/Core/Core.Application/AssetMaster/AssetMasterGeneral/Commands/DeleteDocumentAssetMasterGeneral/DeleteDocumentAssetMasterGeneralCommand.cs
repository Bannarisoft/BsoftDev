using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteDocumentAssetMasterGeneral
{
    public class DeleteDocumentAssetMasterGeneralCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? assetPath { get; set; }       
    }
}