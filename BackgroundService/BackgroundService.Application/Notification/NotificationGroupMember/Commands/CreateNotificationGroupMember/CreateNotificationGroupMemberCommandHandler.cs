using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Entities.Notification;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Commands.CreateNotificationGroupMember
{
    public class CreateNotificationGroupMemberCommandHandler : IRequestHandler<CreateNotificationGroupMemberCommand, int>
    {
        private readonly INotificationGroupMemberCommand _notificationGroupMemberCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public CreateNotificationGroupMemberCommandHandler(INotificationGroupMemberCommand notificationGroupMemberCommand, IMediator imediator, IMapper imapper)
        {
            _notificationGroupMemberCommand = notificationGroupMemberCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<int> Handle(CreateNotificationGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var NotificationGroup = _imapper.Map<NotificationGroupMembers>(request);
            
            var result = await _notificationGroupMemberCommand.CreateAsync(NotificationGroup);
            
            return result > 0 ? result : throw new ExceptionRules("Notification Group Member Creation Failed.");
        }
    }
}