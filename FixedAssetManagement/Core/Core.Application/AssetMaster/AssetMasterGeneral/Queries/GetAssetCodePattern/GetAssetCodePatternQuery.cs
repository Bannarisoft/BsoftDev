using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetCodePattern
{
    public class GetAssetCodePatternQuery : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>> 
    {
        
    }
}