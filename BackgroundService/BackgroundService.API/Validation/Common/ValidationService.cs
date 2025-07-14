
using BackgroundService.API.Validation.NotificationConfig;
using BackgroundService.API.Validation.NotificationLevelHierarchy;
using BackgroundService.API.Validation.NotificationGroup;
using BackgroundService.Application.Notification.NotificationConfig.Command.CreateNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.DeleteNotificationConfig;
using BackgroundService.Application.Notification.NotificationConfig.Command.UpdateNotificationConfig;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationGroup.Commands.CreateNotificationGroup;
using BackgroundService.Application.Notification.NotificationGroup.Commands.DeleteNotificationGroup;
using BackgroundService.Application.Notification.NotificationGroup.Commands.UpdateNotificationGroup;
using FluentValidation;
using BackgroundService.Application.Notification.NotificationTemplate.Command.CreateNotificationTemplate;
using BackgroundService.Application.Notification.NotificationTemplate.Command.UpdateNotificationTemplate;
using BackgroundService.Application.Notification.NotificationTemplate.Command.DeleteNotificationTemplate;
using BackgroundService.API.Validation.NotificationTemplate;

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

            services.AddScoped<IValidator<CreateNotificationGroupCommand>, CreateNotificationGroupCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationGroupCommand>, UpdateNotificationGroupCommandValidator>();
            services.AddScoped<IValidator<DeleteNotificationGroupCommand>, DeleteNotificationGroupCommandValidator>();

            services.AddScoped<IValidator<CreateNotificationTemplateCommand>, CreateNotificationTemplateCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationTemplateCommand>, UpdateNotificationTemplateCommandValidator>();
            services.AddScoped<IValidator<DeleteNotificationTemplateCommand>, DeleteNotificationTemplateCommandValidator>();
        }
    }
}