using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty
{
    public class GetAssetWarrantyQuery : IRequest<ApiResponseDTO<List<AssetWarrantyDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }

}