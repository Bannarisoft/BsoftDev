using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationGroupMembers;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Entities.Notification;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Commands.DeleteNotificationGroupMember
{
    public class DeleteNotificationGroupMemberCommandHandler : IRequestHandler<DeleteNotificationGroupMemberCommand, bool>
    {
         private readonly INotificationGroupMemberCommand _notificationGroupMemberCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public DeleteNotificationGroupMemberCommandHandler(INotificationGroupMemberCommand notificationGroupMemberCommand,IMediator imediator, IMapper imapper)
        {
            _notificationGroupMemberCommand = notificationGroupMemberCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<bool> Handle(DeleteNotificationGroupMemberCommand request, CancellationToken cancellationToken)
        {
            var NotificationGroup = _imapper.Map<NotificationGroupMembers>(request);
            var result = await _notificationGroupMemberCommand.DeleteAsync(request.Id,NotificationGroup);
        
            return result == true ? result : throw new ExceptionRules("Notification Group Member deletion failed.");
        }
    }
}