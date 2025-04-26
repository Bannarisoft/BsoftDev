

using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetRequestType
{
    public class GetRequestTypeQuery : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>> 
    {
        
    }
}