using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarrantyAutoComplete
{
    public class GetAssetWarrantyAutoCompleteQuery : IRequest<ApiResponseDTO<List<AssetWarrantyAutoCompleteDTO>>> 
    {
        public string? SearchPattern { get; set; }
    }
}