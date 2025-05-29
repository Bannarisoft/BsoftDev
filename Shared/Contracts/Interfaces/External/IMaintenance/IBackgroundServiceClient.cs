using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Interfaces.External.IMaintenance
{
    public interface IBackgroundServiceClient
    {
        Task<string> ScheduleWorkOrder(int preventiveScheduleId, int delayInMinutes);
        Task<bool> RemoveHangFireJob(string HangfireJobId);
    }
}