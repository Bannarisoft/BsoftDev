using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundService.Application.Interfaces
{
    public interface IMaintenance
    {
       public Task SchedulerWorkOrderExecute(int PreventiveScheduleId);
    }
}