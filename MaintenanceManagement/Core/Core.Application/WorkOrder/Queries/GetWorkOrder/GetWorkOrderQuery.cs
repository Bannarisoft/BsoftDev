using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class GetWorkOrderQuery  : IRequest<ApiResponseDTO<List<GetWorkOrderByIdDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}