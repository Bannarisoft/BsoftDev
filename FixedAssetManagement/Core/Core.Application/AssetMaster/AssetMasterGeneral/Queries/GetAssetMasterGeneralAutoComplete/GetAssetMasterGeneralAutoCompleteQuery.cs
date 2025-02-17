
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralAutoComplete
{
    public class GetAssetMasterGeneralAutoCompleteQuery  : IRequest<ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>>
    {
        public string? SearchPattern { get; set; }
    }
}