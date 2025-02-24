using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Commands.DeleteAssetWarranty
{
    public class DeleteAssetWarrantyCommand :  IRequest<ApiResponseDTO<AssetWarrantyDTO>>  
    {
         public int Id { get; set; }    
    }
}