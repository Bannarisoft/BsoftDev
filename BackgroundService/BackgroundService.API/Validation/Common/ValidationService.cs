
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
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.CreateNotificationGroupMember;
using BackgroundService.API.Validation.NotificationGroupMember;
using BackgroundService.Application.Notification.NotificationGroupMember.Commands.UpdateNotificationGroupMember;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.CreateNotificationEventRule;
using BackgroundService.API.Validation.NotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.UpdateNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.DeleteNotificationEventRule;
using BackgroundService.Application.MiscTypeMaster.Command.CreateMiscTypeMaster;
using BackgroundService.Application.MiscTypeMaster.Command.DeleteMiscTypeMaster;
using BackgroundService.Application.MiscTypeMaster.Command.UpdateMiscTypeMaster;
using BackgroundService.Application.MiscMaster.Command.CreateMiscMaster;
using BackgroundService.Application.MiscMaster.Command.DeleteMiscMaster;
using BackgroundService.Application.MiscMaster.Command.UpdateMiscMaster;
using BackgroundService.API.Validation.MiscTypeMaster;
using BackgroundService.Application.MiscMaster;
using BackgroundService.API.Validation.MiscMaster;

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

            services.AddScoped<IValidator<CreateNotificationGroupMemberCommand>, CreateNotificationGroupMemberCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationGroupMemberCommand>, UpdateNotificationGroupMemberCommandValidator>();            

            services.AddScoped<IValidator<CreateNotificationEventRuleCommand>, CreateNotificationEventRuleCommandValidator>();
            services.AddScoped<IValidator<UpdateNotificationEventRuleCommand>, UpdateNotificationEventRuleCommandValidator>();
            services.AddScoped<IValidator<DeleteNotificationEventRuleCommand>, DeleteNotificationEventRuleCommandValidator>();
            
            services.AddScoped<IValidator<CreateMiscTypeMasterCommand>, CreateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscTypeMasterCommand>, DeleteMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscTypeMasterCommand>, UpdateMiscTypeMasterCommandValidator>();
            services.AddScoped<IValidator<CreateMiscMasterCommand>, CreateMiscMasterCommandValidator>();
            services.AddScoped<IValidator<DeleteMiscMasterCommand>, DeleteMiscMasterCommandValidator>();
            services.AddScoped<IValidator<UpdateMiscMasterCommand>, UpdateMiscMasterCommandValidator>();
            

        }
    }
}