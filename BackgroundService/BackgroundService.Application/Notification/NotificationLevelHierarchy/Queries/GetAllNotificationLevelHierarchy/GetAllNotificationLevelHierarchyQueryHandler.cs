using AutoMapper;
using BackgroundService.Application.Notification.Common.HttpResponse;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationLevelHierarchy;
using BackgroundService.Domain.Events;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy
{
    public class GetAllNotificationLevelHierarchyQueryHandler : IRequestHandler<GetAllNotificationLevelHierarchyQuery, ApiResponseDTO<List<NotificationLevelHierarchyDto>>>
    {
        private readonly INotificationLevelHierarchyQueryRepository _NotificationLevelHierarchyQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetAllNotificationLevelHierarchyQueryHandler(INotificationLevelHierarchyQueryRepository NotificationLevelHierarchyQueryRepository, IMediator mediator, IMapper mapper)
        {
            _NotificationLevelHierarchyQueryRepository = NotificationLevelHierarchyQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<ApiResponseDTO<List<NotificationLevelHierarchyDto>>> Handle(GetAllNotificationLevelHierarchyQuery request, CancellationToken cancellationToken)
        {
            var (NotificationLevelHierarchy, totalCount) = await _NotificationLevelHierarchyQueryRepository.GetAllNotificationLevelHierarchyAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var NotificationLevelHierarchyDto = _mapper.Map<List<NotificationLevelHierarchyDto>>(NotificationLevelHierarchy);

            // 📘 Log domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetNotificationLevelHierarchy",
                actionCode: "Get",
                actionName: NotificationLevelHierarchy.Count().ToString(),
                details: "Notification details were fetched.",
                module: "NotificationLevelHierarchy"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // ✅ Return
            return new ApiResponseDTO<List<NotificationLevelHierarchyDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = NotificationLevelHierarchyDto,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }



    }
}