using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Entities.Notification;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Commands.DeleteNotificationEventRule
{
    public class DeleteNotificationEventRuleCommandHandler : IRequestHandler<DeleteNotificationEventRuleCommand, bool>
    {
        private readonly INotificationEventRuleCommand _notificationEventRuleCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public DeleteNotificationEventRuleCommandHandler(INotificationEventRuleCommand notificationEventRuleCommand, IMediator imediator, IMapper imapper)
        {
            _notificationEventRuleCommand = notificationEventRuleCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<bool> Handle(DeleteNotificationEventRuleCommand request, CancellationToken cancellationToken)
        {
            var NotificationGroup = _imapper.Map<NotificationEventRule>(request);
            var result = await _notificationEventRuleCommand.DeleteAsync(request.Id,NotificationGroup);
        
            return result == true ? result : throw new ExceptionRules("Notification Event Rule deletion failed.");
        }
    }
}