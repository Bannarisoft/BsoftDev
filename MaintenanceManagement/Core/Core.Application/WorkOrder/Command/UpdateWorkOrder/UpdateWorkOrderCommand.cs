using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommand : IRequest<ApiResponseDTO<WorkOrderUpdateDto>>     
    {
        public WorkOrderUpdateDto? WorkOrder { get; set; }    
    }
}