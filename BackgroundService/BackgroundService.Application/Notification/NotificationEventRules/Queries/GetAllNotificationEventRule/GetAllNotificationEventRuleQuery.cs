using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Notification.Common.HttpResponse;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Queries.GetAllNotificationEventRule
{
    public class GetAllNotificationEventRuleQuery : IRequest<ApiResponseDTO<List<NotificationEventRuleDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}