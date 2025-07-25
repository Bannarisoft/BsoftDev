
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Events;
using MediatR;

namespace  BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy
{
    public class UpdateNotificationLevelHierarchyCommandHandler  : IRequestHandler<UpdateNotificationLevelHierarchyCommand, int>
    {
        private readonly INotificationLevelHierarchyCommandRepository _NotificationLevelHierarchyCommandRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UpdateNotificationLevelHierarchyCommandHandler(INotificationLevelHierarchyCommandRepository NotificationLevelHierarchyCommandRepository, IMediator mediator, IMapper mapper)
        {
            _NotificationLevelHierarchyCommandRepository = NotificationLevelHierarchyCommandRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateNotificationLevelHierarchyCommand request, CancellationToken cancellationToken)
        {       
            var NotificationLevelHierarchy = _mapper.Map<Domain.Entities.Notification.NotificationLevelHierarchy>(request);
            var result = await _NotificationLevelHierarchyCommandRepository.UpdateAsync(request.Id, NotificationLevelHierarchy);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: NotificationLevelHierarchy.Id.ToString(),
                actionName: NotificationLevelHierarchy.Description,
                details: $"Notification Config was updated",
                module: "NotificationLevelHierarchy");
            await _mediator.Publish(domainEvent, cancellationToken);
           
            return result > 0 ? result : throw new ExceptionRules("Notification Config update failed.");   
        }
    }
}