using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class GetAssetMasterGeneralByIdQuery : IRequest<ApiResponseDTO<AssetMasterDTO>>
    {
        public int Id { get; set; }
    }
}