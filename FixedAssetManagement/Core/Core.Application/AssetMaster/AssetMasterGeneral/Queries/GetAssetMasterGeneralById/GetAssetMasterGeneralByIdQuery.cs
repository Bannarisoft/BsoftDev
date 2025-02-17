using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById
{
    public class GetAssetMasterGeneralByIdQuery : IRequest<ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        public int Id { get; set; }
    }
}