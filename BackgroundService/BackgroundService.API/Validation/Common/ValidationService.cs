
using BackgroundService.API.Validation.NotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.CreateNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.DeleteNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.UpdateNotificationConfig;
using FluentValidation;

namespace BackgroundService.API.Validation.Common
{
    public class ValidationService
    {
        public void AddValidationServices(IServiceCollection services)
        {
            services.AddScoped<MaxLengthProvider>();
            services.AddScoped<IValidator<CreateNotificationConfigCommand>, CreateNotificationConfigCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationConfigCommand>, UpdateNotificationConfigCommandValidator>();
            services.AddScoped<IValidator<DeleteNotificationConfigCommand>, DeleteNotificationConfigCommandValidator>();
        }
    }
}