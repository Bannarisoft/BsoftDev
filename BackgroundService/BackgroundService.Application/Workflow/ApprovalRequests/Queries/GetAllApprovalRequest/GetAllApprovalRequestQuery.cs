using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.HttpResponse;
using MediatR;

namespace BackgroundService.Application.Workflow.ApprovalRequests.Queries.GetAllApprovalRequest
{
    public class GetAllApprovalRequestQuery : IRequest<ApiResponseDTO<List<ApprovalRequestDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}