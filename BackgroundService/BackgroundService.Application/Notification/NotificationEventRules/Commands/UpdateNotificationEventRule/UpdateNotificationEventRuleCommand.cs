using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationEventRules.Commands.UpdateNotificationEventRule
{
    public class UpdateNotificationEventRuleCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public int NotificationChannelId { get; set; }
        public int TemplateId { get; set; }
        public int NotificationLevelHierarchyId { get; set; }
        public int RecipientTypeId { get; set; }
        public byte IsActive { get; set; }
    }
}