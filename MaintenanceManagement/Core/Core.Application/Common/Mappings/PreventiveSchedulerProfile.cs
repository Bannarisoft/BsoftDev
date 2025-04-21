using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class PreventiveSchedulerProfile : Profile
    {
        public PreventiveSchedulerProfile()
        {
            CreateMap<CreatePreventiveSchedulerCommand, PreventiveSchedulerHeader>()
            .ForMember(dest => dest.PreventiveSchedulerActivities, opt => opt.MapFrom(src => src.Activity))
            .ForMember(dest => dest.PreventiveSchedulerItems, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

             CreateMap<PreventiveSchedulerActivityDto, PreventiveSchedulerActivity>();
             CreateMap<PreventiveSchedulerItemsDto, PreventiveSchedulerItems>()
             .ForMember(dest => dest.OldItemId, opt => opt.MapFrom(src => src.ItemId))
             .ForMember(dest => dest.ItemId, opt => opt.Ignore());

             CreateMap<UpdatePreventiveSchedulerCommand, PreventiveSchedulerHeader>()
            .ForMember(dest => dest.PreventiveSchedulerActivities, opt => opt.MapFrom(src => src.Activity))
             .ForMember(dest => dest.PreventiveSchedulerItems, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<PreventiveSchedulerActivityUpdateDto, PreventiveSchedulerActivity>();
            CreateMap<PreventiveSchedulerItemUpdateDto, PreventiveSchedulerItems>()
            .ForMember(dest => dest.OldItemId, opt => opt.MapFrom(src => src.ItemId))
             .ForMember(dest => dest.ItemId, opt => opt.Ignore());

            CreateMap<DeletePreventiveSchedulerCommand, PreventiveSchedulerHeader>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));

             CreateMap<PreventiveSchedulerHeader, PreventiveSchedulerHdrByIdDto>()
            .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.PreventiveSchedulerActivities)) 
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PreventiveSchedulerItems))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active ? 1 : 0));

            CreateMap<PreventiveSchedulerActivity, PreventiveSchedulerActivityByIdDto>();
            CreateMap<PreventiveSchedulerItems, PreventiveSchedulerItemByIdDto>()
            .ForMember(dest => dest.OldItemId, opt => opt.MapFrom(src => src.OldItemId));

            CreateMap<Core.Domain.Entities.MachineMaster, PreventiveSchedulerDetail>()
            .ForMember(dest => dest.MachineId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PreventiveSchedulerId, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrderCreationStartDate, opt => opt.Ignore())
            .ForMember(dest => dest.ActualWorkOrderDate, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialReqStartDays, opt => opt.Ignore())
            .ForMember(dest => dest.RescheduleReason, opt => opt.Ignore());
        }
    }
}