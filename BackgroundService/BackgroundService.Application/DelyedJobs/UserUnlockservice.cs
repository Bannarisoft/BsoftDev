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
        public UserUnlockservice(IUserUnlockService userUnlockService)
        {
            _userUnlockService = userUnlockService;
        }
        
         public async Task Execute(string userName)
        {
            await _userUnlockService.UnlockUser(userName);
        }
    }
}