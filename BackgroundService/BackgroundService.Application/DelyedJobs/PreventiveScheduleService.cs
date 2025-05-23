using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces;

namespace BackgroundService.Application.DelyedJobs
{
    public class PreventiveScheduleService
    {
        private readonly IMaintenance _maintenance;
        public PreventiveScheduleService(IMaintenance maintenance)
        {
            _maintenance = maintenance;
        }
        
        
        //  public async Task ScheduleworkOrderExecute(int PreventiveScheduleId)
        // {
        //     await _maintenance.SchedulerWorkOrderExecute(PreventiveScheduleId);
        // }
    }
}