using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationDetail;
using BackgroundService.Domain.Events;
using MediatR;

namespace BackgroundService.Application.Notification.GetNotificationDetail.GetNotificationDetailById
{
    public class GetNotificationDetailByUserIdHandler : IRequestHandler<GetNotificationDetailByUserId, List<GetNotificationDetailDto>>
    {
        private readonly INotificationDetailRepository _notificationDetailRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetNotificationDetailByUserIdHandler(
            INotificationDetailRepository notificationDetailRepository,
            IMediator mediator,
            IMapper mapper)
        {
            _notificationDetailRepository = notificationDetailRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<List<GetNotificationDetailDto>> Handle(GetNotificationDetailByUserId request, CancellationToken cancellationToken)
        {
            var entities = await _notificationDetailRepository.GetAllByUserIdAsync(request.UserId);

            if (entities == null || !entities.Any())
                throw new KeyNotFoundException($"No notification records found for UserId {request.UserId}");

            var result = _mapper.Map<List<GetNotificationDetailDto>>(entities);

            // Domain Event
            await _mediator.Publish(new AuditLogsDomainEvent(
                actionDetail: "GetAllByUserId",
                actionCode: "GetNotificationDetailByUserIdQuery",
                actionName: request.UserId,
                details: $"Fetched {result.Count} notifications for UserId {request.UserId}.",
                module: "NotificationDetail"), cancellationToken);

            return result;
        }
    }
    

}