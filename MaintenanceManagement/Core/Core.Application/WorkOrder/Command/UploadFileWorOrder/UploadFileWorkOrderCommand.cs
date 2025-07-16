using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.WorkOrder.Command.UploadFileWorOrder
{
    public class UploadFileWorkOrderCommand : IRequest<ApiResponseDTO<WorkOrderImageDto>>
    {
         public IFormFile? File { get; set; }       
    }
}