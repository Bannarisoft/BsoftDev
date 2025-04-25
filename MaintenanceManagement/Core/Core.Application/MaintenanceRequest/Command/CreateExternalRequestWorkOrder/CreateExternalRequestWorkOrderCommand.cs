using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using MediatR;

namespace Core.Application.MaintenanceRequest.Command.CreateExternalRequestWorkOrder
{
    public class CreateExternalRequestWorkOrderCommand  : IRequest<ApiResponseDTO<List<int>>>
    {
    public List<int>? Ids { get; set; } 
      

        
    }
}