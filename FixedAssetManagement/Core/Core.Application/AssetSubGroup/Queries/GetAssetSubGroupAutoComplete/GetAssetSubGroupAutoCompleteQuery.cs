using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Application.AssetSubGroup.Queries.GetAssetSubGroup;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetSubGroup.Queries.GetAssetSubGroupAutoComplete
{
    public class GetAssetSubGroupAutoCompleteQuery : IRequest<ApiResponseDTO<List<AssetSubGroupAutoCompleteDTO>>>
    {
        public string? SearchPattern { get; set; }
    }
}