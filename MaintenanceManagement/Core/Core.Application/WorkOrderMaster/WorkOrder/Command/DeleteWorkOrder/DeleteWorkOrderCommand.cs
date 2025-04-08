using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrder;
using MediatR;

namespace Core.Application.WorkOrderMaster.WorkOrder.Command.DeleteWorkOrder
{
    public class DeleteWorkOrderCommand  :  IRequest<ApiResponseDTO<WorkOrderCombineDto>> 
    {
          public int Id { get; set; }     
    }
}