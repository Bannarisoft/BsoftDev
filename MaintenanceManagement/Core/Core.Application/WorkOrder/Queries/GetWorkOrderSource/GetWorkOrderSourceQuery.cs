
using Core.Application.Common.HttpResponse;
using Core.Application.MiscMaster.Queries.GetMiscMaster;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrderSource
{
    public class GetWorkOrderSourceQuery : IRequest<ApiResponseDTO<List<GetMiscMasterDto>>> 
    {
        
    }
}