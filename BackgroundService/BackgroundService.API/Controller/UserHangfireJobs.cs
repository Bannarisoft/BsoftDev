using BackgroundService.Application.DelyedJobs;
using BackgroundService.Application.Interfaces;
using BackgroundService.Infrastructure.Jobs;
using BackgroundService.Infrastructure.Services;
using Contracts.Events.Notifications;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundService.API.Controllers
{
    [ApiController]
    [Route("api/userhangfirejobs")]
    public class UserHangfireJobsController : ControllerBase
    {
        [HttpPost("user-verification-code-removal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ScheduleVerificationCodeRemoval([FromBody] ScheduleRemoveCodeCommand command)
        {
            BackgroundJob.Schedule<IVerificationCodeCleanupService>(
                service => service.RemoveVerificationCode(command.UserName, command.DelayInMinutes),
                TimeSpan.FromMinutes(command.DelayInMinutes)
            );

            return Ok("Verification code removal scheduled successfully.");
        }

        [HttpPost("user-unlock")]        
        public IActionResult ScheduleUserUnlock([FromBody] ScheduleRemoveCodeCommand command)
        {
           /*  BackgroundJob.Schedule<UserUnlockBackgroundJob>(
               job => job.Execute(command.UserName),
                TimeSpan.FromMinutes(command.DelayInMinutes)
            ); */
            BackgroundJob.Schedule<UserUnlockservice>(
                 job => job.Execute(command.UserName),
                TimeSpan.FromMinutes(1)
            );
            return Ok("User unlock scheduled successfully.");   
        }
    }
}
