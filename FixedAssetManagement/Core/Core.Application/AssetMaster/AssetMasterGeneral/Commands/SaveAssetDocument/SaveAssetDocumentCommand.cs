using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.SaveAssetDocument
{
    public class SaveAssetDocumentCommand :  IRequest<ApiResponseDTO<bool>>
    {
        public int Id { get; set; }
        public string? AssetCode { get; set; }  
        public string? assetPath { get; set; }          

    }
}
