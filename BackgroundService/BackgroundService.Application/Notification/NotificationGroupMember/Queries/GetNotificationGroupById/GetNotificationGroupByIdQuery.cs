using MediatR;
using BackgroundService.Application.Notification.Common.HttpResponse;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Queries.GetNotificationGroupById
{
    public class GetNotificationGroupByIdQuery : IRequest<ApiResponseDTO<NotificationGroupDto>>
    {
        public int Id { get; set; }
    }
}
