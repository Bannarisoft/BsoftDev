using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.UpdateWorkOrder
{
    public class UpdateWorkOrderCommand : IRequest<ApiResponseDTO<bool>>     
    {
        public WorkOrderUpdateDto? WorkOrder { get; set; }    
    }
}