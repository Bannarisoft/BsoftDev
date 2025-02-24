using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetWarrantyClaimStatus
{
    public class GetWarrantyClaimStatusQuery : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>> 
    {
        
    }
}