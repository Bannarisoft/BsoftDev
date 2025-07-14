using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy;
using BackgroundService.Domain.Events;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetNotificationLevelHierarchyById
{
    public class GetNotificationLevelHierarchyByIdQueryHandler : IRequestHandler<GetNotificationLevelHierarchyByIdQuery, NotificationLevelHierarchyDto>
    {
        private readonly INotificationLevelHierarchyQueryRepository _NotificationLevelHierarchyQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetNotificationLevelHierarchyByIdQueryHandler(INotificationLevelHierarchyQueryRepository NotificationLevelHierarchyQueryRepository, IMediator mediator, IMapper mapper)
        {
            _NotificationLevelHierarchyQueryRepository = NotificationLevelHierarchyQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<NotificationLevelHierarchyDto> Handle(GetNotificationLevelHierarchyByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _NotificationLevelHierarchyQueryRepository.GetByIdAsync(request.Id);     
            if (result == null)
            {
                throw new KeyNotFoundException($"NotificationLevelHierarchy with Id {request.Id} not found.");
            }       
            var NotificationLevelHierarchy = _mapper.Map<NotificationLevelHierarchyDto>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "GetNotificationLevelHierarchyByIdQuery",
                actionName: NotificationLevelHierarchy.Id.ToString(),
                details: $"NotificationLevelHierarchy details {NotificationLevelHierarchy.Id} was fetched.",
                module: "NotificationLevelHierarchy"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return NotificationLevelHierarchy;
        }

    }
}