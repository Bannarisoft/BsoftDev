
using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using MediatR;

namespace Core.Application.WorkOrder.Command.CreateWorkOrder
{
    public class CreateWorkOrderCommand : IRequest<ApiResponseDTO<WorkOrderCombineDto>>  
    {
       public WorkOrderCombineDto? WorkOrder { get; set; }       
    }
}