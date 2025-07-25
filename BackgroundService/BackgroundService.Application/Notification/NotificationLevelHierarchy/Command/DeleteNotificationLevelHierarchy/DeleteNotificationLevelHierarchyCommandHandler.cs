using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Events;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy
{
    public class DeleteNotificationLevelHierarchyCommandHandler : IRequestHandler<DeleteNotificationLevelHierarchyCommand, int>
    {
        private readonly INotificationLevelHierarchyCommandRepository _NotificationLevelHierarchyCommandRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public DeleteNotificationLevelHierarchyCommandHandler(INotificationLevelHierarchyCommandRepository NotificationLevelHierarchyCommandRepository, IMediator mediator, IMapper mapper)
        {
            _NotificationLevelHierarchyCommandRepository = NotificationLevelHierarchyCommandRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<int> Handle(DeleteNotificationLevelHierarchyCommand request, CancellationToken cancellationToken)
        {            
            var NotificationLevelHierarchy = _mapper.Map<Domain.Entities.Notification.NotificationLevelHierarchy>(request);
            var result = await _NotificationLevelHierarchyCommandRepository.DeleteAsync(request.Id,NotificationLevelHierarchy);          

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: NotificationLevelHierarchy.Id.ToString(),
                actionName: NotificationLevelHierarchy.Description ??string.Empty,
                details: $"Notification Config  details was deleted",
                module: "NotificationLevelHierarchy ");
            await _mediator.Publish(domainEvent);
            return result > 0 ? result : throw new ExceptionRules("Notification Config was not found.");
        }


    }
}