using Core.Application.Common.HttpResponse;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using MediatR;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class GetWorkOrderQuery   : IRequest<ApiResponseDTO<List<Dictionary<string, List<GetWorkOrderDto>>>>>
    {
        public DateTimeOffset? fromDate {get; set;}
        public DateTimeOffset? toDate {get; set;}
        public int requestType {get; set;}
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SearchTerm { get; set; }
    }
}