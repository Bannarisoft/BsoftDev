using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BackgroundService.Application.Notification.NotificationGroup.Commands.CreateNotificationGroup;
using BackgroundService.Application.Notification.NotificationGroup.Commands.DeleteNotificationGroup;
using BackgroundService.Application.Notification.NotificationGroup.Commands.UpdateNotificationGroup;
using static BackgroundService.Domain.Common.BaseEntity;

namespace BackgroundService.Application.Notification.Common.Mappings
{
    public class NotificationGroupProfile : Profile
    {
        public NotificationGroupProfile()
        {
            CreateMap<CreateNotificationGroupCommand, Domain.Entities.Notification.NotificationGroup>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            CreateMap<UpdateNotificationGroupCommand, Domain.Entities.Notification.NotificationGroup>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==  1 ? Status.Active : Status.Inactive));
            CreateMap<DeleteNotificationGroupCommand, Domain.Entities.Notification.NotificationGroup>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));  
        }
        
    }
}