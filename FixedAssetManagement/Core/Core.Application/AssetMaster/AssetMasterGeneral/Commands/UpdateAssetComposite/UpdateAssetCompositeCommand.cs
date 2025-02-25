using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetComposite
{
    public class UpdateAssetCompositeCommand : IRequest<ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        public UpdateAssetCompositeDto AssetComposite { get; set; } = null!;
    }
}