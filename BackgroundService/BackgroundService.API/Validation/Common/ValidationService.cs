
using BackgroundService.API.Validation.NotificationConfig;
using BackgroundService.API.Validation.NotificationGroup;
using BackgroundService.Application.Notification.NotificationConfig.Command.CreateNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.DeleteNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.UpdateNotificationConfig;
using BackgroundService.Application.Notification.NotificationGroup.Commands.CreateNotificationGroup;
using BackgroundService.Application.Notification.NotificationGroup.Commands.DeleteNotificationGroup;
using BackgroundService.Application.Notification.NotificationGroup.Commands.UpdateNotificationGroup;
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

            services.AddScoped<IValidator<CreateNotificationGroupCommand>, CreateNotificationGroupCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationGroupCommand>, UpdateNotificationGroupCommandValidator>();
            services.AddScoped<IValidator<DeleteNotificationGroupCommand>, DeleteNotificationGroupCommandValidator>();
        }
    }
}