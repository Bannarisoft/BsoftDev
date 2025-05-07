using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MaintenanceRequest.Queries.RequestReport
{
    public class RequestReportQuery : IRequest<ApiResponseDTO<List<RequestReportDto>>> 
    {
         public DateTimeOffset? RequestFromDate { get; set; }
        public DateTimeOffset? RequestToDate { get; set; }
        public int GetRequestType { get; set; }
        public int RequestStatus { get; set; }
    }
}