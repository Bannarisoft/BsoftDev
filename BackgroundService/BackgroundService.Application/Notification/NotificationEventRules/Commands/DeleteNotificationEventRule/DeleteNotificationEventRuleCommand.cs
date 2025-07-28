using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Commands.DeleteNotificationEventRule
{
    public class DeleteNotificationEventRuleCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}