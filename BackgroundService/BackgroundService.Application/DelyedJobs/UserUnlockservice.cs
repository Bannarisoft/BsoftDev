using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.Interfaces;

namespace BackgroundService.Application.DelyedJobs
{
    public class UserUnlockservice
    {
        private readonly IUserUnlockService _userUnlockService;
        private readonly IMaintenance _maintenance;
        public UserUnlockservice(IUserUnlockService userUnlockService,IMaintenance maintenance)
        {
            _userUnlockService = userUnlockService;
            _maintenance = maintenance;
        }
        
         public async Task Execute(string userName)
        {
            await _userUnlockService.UnlockUser(userName);
        }
          public async Task ScheduleworkOrderExecute(int PreventiveScheduleId)
        {
            await _maintenance.SchedulerWorkOrderExecute(PreventiveScheduleId);
        }
    }
}