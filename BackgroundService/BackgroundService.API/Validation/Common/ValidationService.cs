
using BackgroundService.API.Validation.NotificationConfig;
using BackgroundService.API.Validation.NotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationConfig.Command.CreateNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.DeleteNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.UpdateNotificationConfig;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy;
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
            services.AddScoped<IValidator<CreateNotificationLevelHierarchyCommand>, CreateNotificationLevelHierarchyCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationLevelHierarchyCommand>, UpdateNotificationLevelHierarchyCommandValidator>();
            services.AddScoped<IValidator<DeleteNotificationLevelHierarchyCommand>, DeleteNotificationLevelHierarchyCommandValidator>();
        }
    }
}