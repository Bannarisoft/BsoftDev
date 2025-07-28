using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.CreateNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.DeleteNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Commands.UpdateNotificationEventRule;
using BackgroundService.Application.Notification.NotificationEventRules.Queries.GetAllNotificationEventRule;
using BackgroundService.Domain.Entities.Notification;
using static BackgroundService.Domain.Common.BaseEntity;

namespace BackgroundService.Application.Notification.Common.Mappings
{
    public class NotificationEventRuleProfile : Profile
    {
        public NotificationEventRuleProfile()
        {
            CreateMap<NotificationEventRule, NotificationEventRuleDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active ? 1 : 0));
            CreateMap<CreateNotificationEventRuleCommand, NotificationEventRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())     
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateNotificationEventRuleCommand, NotificationEventRule>()    
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));            


              CreateMap<DeleteNotificationEventRuleCommand, NotificationEventRule>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));    
        }
    }
}