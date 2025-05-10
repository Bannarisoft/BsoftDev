using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackgroundService.Application.DelyedJobs;
using Contracts.Events.Notifications;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controller
{
    [ApiController]
    [Route("api/maintenancehangfirejobs")]
    public class ManitenanceJobsController : ControllerBase
    {
          [HttpPost("scheduleWorkOrder")]        
        public IActionResult SchedulePreventive([FromBody] ScheduleWorkOrderBackgroundCommand command)
        {
           /*  BackgroundJob.Schedule<UserUnlockBackgroundJob>(
               job => job.Execute(command.UserName),
                TimeSpan.FromMinutes(command.DelayInMinutes)
            ); */
          string newJobId = BackgroundJob.Schedule<UserUnlockservice>(
                 job => job.ScheduleworkOrderExecute(command.PreventiveScheduleId),
                TimeSpan.FromMinutes(1)
            );
            return Ok(new { Message = "Work order scheduled", JobId = newJobId });   
        }
    }
}