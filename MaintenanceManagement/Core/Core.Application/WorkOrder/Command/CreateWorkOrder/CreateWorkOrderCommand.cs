
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class CreateWorkOrderCommand : IRequest<ApiResponseDTO<WorkOrderCombineDto>>  
    {
       public WorkOrderCombineDto? WorkOrderDto { get; set; }       
    }
}