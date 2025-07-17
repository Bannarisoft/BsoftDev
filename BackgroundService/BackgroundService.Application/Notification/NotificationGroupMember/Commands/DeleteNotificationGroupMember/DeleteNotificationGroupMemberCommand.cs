using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Commands.DeleteNotificationGroupMember
{
    public class DeleteNotificationGroupMemberCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}