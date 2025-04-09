using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommand : IRequest<ApiResponseDTO<WorkOrderCombineDto>>     
    {
        public WorkOrderCombineDto? WorkOrder { get; set; }    
    }
}