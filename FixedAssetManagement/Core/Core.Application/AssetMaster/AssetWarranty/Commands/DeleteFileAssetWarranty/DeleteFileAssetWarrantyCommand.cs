using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.DeleteFileAssetWarranty
{
    public class DeleteFileAssetWarrantyCommand : IRequest<bool>
    {
        public string? assetPath { get; set; }
    }
}