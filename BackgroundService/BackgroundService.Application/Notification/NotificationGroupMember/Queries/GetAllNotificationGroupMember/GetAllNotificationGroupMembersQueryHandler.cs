using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.HttpResponse;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Queries.GetAllNotificationGroupMember
{
    public class GetAllNotificationGroupMembersQueryHandler : IRequestHandler<GetAllNotificationGroupMembersQuery, ApiResponseDTO<List<NotificationGroupMemberDto>>>
    {
        private readonly INotificationGroupMemberQuery _notificationGroupMemberQuery;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public GetAllNotificationGroupMembersQueryHandler(INotificationGroupMemberQuery notificationGroupMemberQuery, IMediator mediator, IMapper mapper)
        {
            _notificationGroupMemberQuery = notificationGroupMemberQuery;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<List<NotificationGroupMemberDto>>> Handle(GetAllNotificationGroupMembersQuery request, CancellationToken cancellationToken)
        {
            var (NotificationGroup, TotalCount) = await _notificationGroupMemberQuery.GetAllNotificationGroupAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var NotificationGroupDto = _mapper.Map<List<NotificationGroupMemberDto>>(NotificationGroup);


            return new ApiResponseDTO<List<NotificationGroupMemberDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = NotificationGroupDto,
                TotalCount = TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}