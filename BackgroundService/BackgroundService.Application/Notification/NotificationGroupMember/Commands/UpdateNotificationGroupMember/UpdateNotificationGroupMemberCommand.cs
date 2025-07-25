using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BackgroundService.Application.Notification.NotificationGroupMember.Commands.UpdateNotificationGroupMember
{
    public class UpdateNotificationGroupMemberCommand : IRequest<bool>
    {
         public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public byte IsActive { get; set; }
    }
}