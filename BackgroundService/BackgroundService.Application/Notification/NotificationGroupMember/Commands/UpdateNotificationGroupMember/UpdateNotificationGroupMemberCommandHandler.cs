using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Entities.Notification;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Commands.UpdateNotificationGroupMember
{
    public class UpdateNotificationGroupMemberCommandHandler : IRequestHandler<UpdateNotificationGroupMemberCommand, bool>
    {
        private readonly INotificationGroupMemberCommand _notificationGroupMemberCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public UpdateNotificationGroupMemberCommandHandler(INotificationGroupMemberCommand notificationGroupMemberCommand, IMediator imediator, IMapper imapper)
        {
            _notificationGroupMemberCommand = notificationGroupMemberCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<bool> Handle(UpdateNotificationGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var NotificationGroup = _imapper.Map<NotificationGroupMembers>(request);
            var result = await _notificationGroupMemberCommand.UpdateAsync(NotificationGroup);
            
           
            return result == true ? result : throw new ExceptionRules("Notification Group Member update failed.");
        }
    }
}