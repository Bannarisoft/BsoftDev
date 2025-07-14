using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Entities.Notification;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Commands.UpdateNotificationEventRule
{
    public class UpdateNotificationEventRuleCommandHandler : IRequestHandler<UpdateNotificationEventRuleCommand, bool>
    {
         private readonly INotificationEventRuleCommand _notificationEventRuleCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public UpdateNotificationEventRuleCommandHandler(INotificationEventRuleCommand notificationEventRuleCommand, IMediator imediator, IMapper imapper)
        {
            _notificationEventRuleCommand = notificationEventRuleCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<bool> Handle(UpdateNotificationEventRuleCommand request, CancellationToken cancellationToken)
        {
            var Notification = _imapper.Map<NotificationEventRule>(request);
            var result = await _notificationEventRuleCommand.UpdateAsync(Notification);
            
           
            return result == true ? result : throw new ExceptionRules("Notification Event Rule update failed.");   
        }
    }
}