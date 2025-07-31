using AutoMapper;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.CreateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.DeleteNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Command.UpdateNotificationLevelHierarchy;
using BackgroundService.Application.Notification.NotificationLevelHierarchy.Queries.GetAllNotificationLevelHierarchy;
using static BackgroundService.Domain.Common.BaseEntity;

namespace BackgroundService.Application.Notification.Common.Mappings
{
    public class NotificationLevelHierarchyProfile : Profile
    {
        public NotificationLevelHierarchyProfile()
        {
           CreateMap<Domain.Entities.Notification.NotificationLevelHierarchy,NotificationLevelHierarchyDto>();
            CreateMap<CreateNotificationLevelHierarchyCommand, Domain.Entities.Notification.NotificationLevelHierarchy>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationConfigId, opt => opt.MapFrom(src => src.NotificationConfigId))
                .ForMember(dest => dest.TargetTypeId, opt => opt.MapFrom(src => src.TargetTypeId))
                .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.TargetId))
                .ForMember(dest => dest.ApprovalModeId, opt => opt.MapFrom(src => src.ApprovalModeId))                            
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))      
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateNotificationLevelHierarchyCommand, Domain.Entities.Notification.NotificationLevelHierarchy>()
                .ForMember(dest => dest.NotificationConfigId, opt => opt.MapFrom(src => src.NotificationConfigId))
                .ForMember(dest => dest.TargetTypeId, opt => opt.MapFrom(src => src.TargetTypeId))
                .ForMember(dest => dest.TargetId, opt => opt.MapFrom(src => src.TargetId))
                .ForMember(dest => dest.ApprovalModeId, opt => opt.MapFrom(src => src.ApprovalModeId))                
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))      
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

              CreateMap<DeleteNotificationLevelHierarchyCommand, Domain.Entities.Notification.NotificationLevelHierarchy>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));    
        }
    }
}