using Contracts.Events.Notifications;
using BackgroundService.Application.Interfaces;
using BackgroundService.Application;

namespace BackgroundService.Infrastructure.Services
{
    public class VerificationCodeCleanupService : IVerificationCodeCleanupService
    {
        public Task RemoveVerificationCode(string userName, int delayInMinutes)
        {
            ForgotPasswordCache.RemoveVerificationCode(userName);
            return Task.CompletedTask;
        }
    }
}       