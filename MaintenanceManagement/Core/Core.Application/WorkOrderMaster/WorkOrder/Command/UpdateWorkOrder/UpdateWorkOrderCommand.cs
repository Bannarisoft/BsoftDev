using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using MediatR;

namespace Core.Application.WorkOrderMaster.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommand : IRequest<ApiResponseDTO<WorkOrderCombineDto>>     
    {
        public WorkOrderCombineDto? WorkOrder { get; set; }    
    }
}