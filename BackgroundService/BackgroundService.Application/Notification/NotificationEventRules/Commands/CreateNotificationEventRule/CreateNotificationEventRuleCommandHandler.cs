using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.Common.Interfaces.INotificationEventRule;
using BackgroundService.Application.Notification.Exceptions;
using BackgroundService.Domain.Entities.Notification;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Commands.CreateNotificationEventRule
{
    public class CreateNotificationEventRuleCommandHandler : IRequestHandler<CreateNotificationEventRuleCommand, int>
    {
        private readonly INotificationEventRuleCommand _notificationEventRuleCommand;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public CreateNotificationEventRuleCommandHandler(INotificationEventRuleCommand notificationEventRuleCommand, IMediator imediator, IMapper imapper)
        {
            _notificationEventRuleCommand = notificationEventRuleCommand;
            _imediator = imediator;
            _imapper = imapper;
        }
        public async Task<int> Handle(CreateNotificationEventRuleCommand request, CancellationToken cancellationToken)
        {
            var Notification = _imapper.Map<NotificationEventRule>(request);
            
            var result = await _notificationEventRuleCommand.CreateAsync(Notification);
            
            return result > 0 ? result : throw new ExceptionRules("Notification Event Rule Creation Failed.");
        }
    }
}