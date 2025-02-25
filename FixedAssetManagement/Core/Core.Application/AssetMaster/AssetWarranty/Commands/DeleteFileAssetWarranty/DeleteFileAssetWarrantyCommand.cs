using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.DeleteFileAssetWarranty
{
    public class DeleteFileAssetWarrantyCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? AssetCode { get; set; }
    }
}