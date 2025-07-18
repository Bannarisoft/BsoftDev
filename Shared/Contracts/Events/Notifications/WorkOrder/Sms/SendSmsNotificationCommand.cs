using System.Collections.Generic;
using MediatR;

namespace Contracts.Events.Notifications.WorkOrder.Sms
{
    public class SendSmsNotificationCommand : IRequest<bool>
    {
        public List<string>  mobileNumbers { get; set; }    
        public string? message { get; set; }        
    }
}