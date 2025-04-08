
using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using MediatR;

namespace Core.Application.WorkOrderMaster.WorkOrder.Command.CreateWorkOrder
{
    public class CreateWorkOrderCommand : IRequest<ApiResponseDTO<Core.Domain.Entities.WorkOrderMaster.WorkOrder>>  
    {
       public WorkOrderCombineDto? AssetMaster { get; set; }       
    }
}