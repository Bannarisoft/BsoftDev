using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteFileAssetMasterGeneral
{
    public class DeleteFileAssetMasterGeneralCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? assetPath { get; set; }
        public string? CompanyName { get; set; }  
        public string? UnitName { get; set; } 
    }
}