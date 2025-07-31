using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.HttpResponse;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Queries.GetAllNotificationEventRule
{
    public class GetAllNotificationEventRuleQueryHandler : IRequestHandler<GetAllNotificationEventRuleQuery, ApiResponseDTO<List<NotificationEventRuleDto>>>
    {
        private readonly INotificationEventRuleQuery _notificationEventRuleQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetAllNotificationEventRuleQueryHandler(INotificationEventRuleQuery notificationEventRuleQuery, IMediator mediator, IMapper mapper)
        {
            _notificationEventRuleQuery = notificationEventRuleQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<NotificationEventRuleDto>>> Handle(GetAllNotificationEventRuleQuery request, CancellationToken cancellationToken)
        {
            var (Notification, TotalCount) = await _notificationEventRuleQuery.GetAllNotificationEventRuleAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var NotificationDto = _mapper.Map<List<NotificationEventRuleDto>>(Notification);


            return new ApiResponseDTO<List<NotificationEventRuleDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = NotificationDto,
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}