using BackgroundService.Application.Notification.Common.HttpResponse;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy
{
    public class GetAllNotificationLevelHierarchyQuery : IRequest<ApiResponseDTO<List<NotificationLevelHierarchyDto>>>
    {        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SearchTerm { get; set; }
    }
}