using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.INotifications
{
    public interface IRabbitMqPublisher
    {
        void PublishMessage<T>(T message, bool isUserCreated = false);
    }
}