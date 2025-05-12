using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Common.Interfaces.IBackgroundService
{
    public interface IBackgroundServiceClient
    {
        Task<string> ScheduleWorkOrder(int PreventiveScheduleId, int delayInMinutes);
    }
}