using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterByIdSplit
{
    public class GetAssetMasterByIdSplitQuery : IRequest<ApiResponseDTO<AssetMasterSplitDto>>
    {
        public int Id { get; set; }
    }
}