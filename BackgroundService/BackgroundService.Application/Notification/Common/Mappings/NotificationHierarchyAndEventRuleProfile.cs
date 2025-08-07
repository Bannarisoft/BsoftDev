using AutoMapper;
using BackgroundService.Domain.Entities.Notification;
using BackgroundService.Application.Notification.NotificationHierarchyAndEventRule.Commands.UpdateNotificationEventRule;
using BackgroundService.Application.Notification.NotificationHierarchyAndEventRule.DTOs;

namespace BackgroundService.Application.Notification.Common.Mappings
{
    public class NotificationHierarchyAndEventRuleProfile : Profile
    {
        public NotificationHierarchyAndEventRuleProfile()
        {
            // Map from Command DTO to NotificationLevelHierarchy (for insert/update)
            CreateMap<NotificationHierarchyAndEventRuleDto, NotificationLevelHierarchy>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id should be set manually
                .ForMember(dest => dest.NotificationEventRules, opt => opt.Ignore()); // handled separately

            // Map from NotificationEventRuleDto to NotificationEventRule entity
            CreateMap<NotificationEventRuleDto, NotificationEventRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is not required for new
                .ForMember(dest => dest.NotificationLevelHierarchy, opt => opt.Ignore()) // avoid circular reference
                .ForMember(dest => dest.Channel, opt => opt.Ignore())
                .ForMember(dest => dest.RecipientType, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationTemplates, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationEventLog, opt => opt.Ignore());

            // Reverse mapping (optional, for GetById handler return)
            CreateMap<NotificationLevelHierarchy, NotificationHierarchyAndEventRuleDto>()
                .ForMember(dest => dest.NotificationEventRules, opt => opt.MapFrom(src => src.NotificationEventRules));

            CreateMap<NotificationEventRule, NotificationEventRuleDto>();
        }
    }
}
