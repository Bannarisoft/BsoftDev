using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Interfaces.Notification
{
    public interface INotificationDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}