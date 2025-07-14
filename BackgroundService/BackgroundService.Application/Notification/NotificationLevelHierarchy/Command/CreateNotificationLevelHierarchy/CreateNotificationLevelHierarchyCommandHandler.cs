using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Events;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy
{
    public class CreateNotificationLevelHierarchyCommandHandler : IRequestHandler<CreateNotificationLevelHierarchyCommand, int>
    {
        private readonly INotificationLevelHierarchyCommandRepository _NotificationLevelHierarchyRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CreateNotificationLevelHierarchyCommandHandler(INotificationLevelHierarchyCommandRepository NotificationLevelHierarchyRepository, IMediator mediator, IMapper mapper)
        {
            _NotificationLevelHierarchyRepository = NotificationLevelHierarchyRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateNotificationLevelHierarchyCommand request, CancellationToken cancellationToken)
        {
            var NotificationLevelHierarchy = _mapper.Map<Domain.Entities.Notification.NotificationLevelHierarchy>(request);
            var result = await _NotificationLevelHierarchyRepository.CreateAsync(NotificationLevelHierarchy);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: NotificationLevelHierarchy.Id.ToString(),
                actionName: NotificationLevelHierarchy.Description??string.Empty,
                details: $"Notification Level Hierarchy details was created",
                module: "NotificationLevelHierarchy");
            await _mediator.Publish(domainEvent, cancellationToken);

            return result > 0 ? result : throw new ExceptionRules("Notification Level Hierarchy  Creation Failed.");
        }
    }

}
