using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.PreventiveSchedulers.Commands.ActiveInActivePreventive;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler;
using Core.Application.PreventiveSchedulers.Queries.GetPreventiveSchedulerById;
using Core.Domain.Entities;
using Core.Domain.Entities.WorkOrderMaster;
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
            .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
            .ForMember(dest => dest.UnitId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

             CreateMap<PreventiveSchedulerActivityDto, PreventiveSchedulerActivity>();
             CreateMap<PreventiveSchedulerItemsDto, PreventiveSchedulerItems>()
             .ForMember(dest => dest.OldItemId, opt => opt.MapFrom(src => src.ItemId))
             .ForMember(dest => dest.ItemId, opt => opt.Ignore());

             CreateMap<UpdatePreventiveSchedulerCommand, PreventiveSchedulerHeader>()
            .ForMember(dest => dest.PreventiveSchedulerActivities, opt => opt.MapFrom(src => src.Activity))
             .ForMember(dest => dest.PreventiveSchedulerItems, opt => opt.MapFrom(src => src.Items));
            // .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

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
            .ForMember(dest => dest.PreventiveSchedulerHeaderId, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrderCreationStartDate, opt => opt.Ignore())
            .ForMember(dest => dest.ActualWorkOrderDate, opt => opt.Ignore())
            .ForMember(dest => dest.MaterialReqStartDays, opt => opt.Ignore())
            .ForMember(dest => dest.RescheduleReason, opt => opt.Ignore());

            CreateMap<PreventiveSchedulerHeader, Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PreventiveScheduleId, opt => opt.MapFrom((src, dest, destMember, ctx) =>
                ctx.Items.ContainsKey("PreventiveSchedulerDetailId") ? (int)ctx.Items["PreventiveSchedulerDetailId"] : 0))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom((src, dest, destMember, ctx) =>
                ctx.Items.ContainsKey("StatusId") ? (int)ctx.Items["StatusId"] : 0))
            // .ForMember(dest => dest.WorkOrderDocNo, opt => opt.MapFrom((src, dest, destMember, ctx) =>
            //     ctx.Items.ContainsKey("WorkOrderDocNo")))
            // .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => _ipAddressService.GetCompanyId()))
            // .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => _ipAddressService.GetUnitId()))
            .ForMember(dest => dest.WorkOrderActivities, opt => opt.MapFrom(src => src.PreventiveSchedulerActivities))
            .ForMember(dest => dest.WorkOrderItems, opt => opt.MapFrom(src => src.PreventiveSchedulerItems))
            .ForMember(dest => dest.DowntimeStart, opt => opt.Ignore())
            .ForMember(dest => dest.DowntimeEnd, opt => opt.Ignore())
            .ForMember(dest => dest.RootCauseId, opt => opt.Ignore());

            CreateMap<PreventiveSchedulerActivity, WorkOrderActivity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrderId, opt => opt.Ignore())
            .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.ActivityId));

              CreateMap<PreventiveSchedulerItems, WorkOrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkOrderId, opt => opt.Ignore())
            // .ForMember(dest => dest.Sou, opt => opt.MapFrom(src => src.SourceId))
            .ForMember(dest => dest.OldItemCode, opt => opt.MapFrom(src => src.OldItemId));
            CreateMap<PreventiveSchedulerDetail, PreventiveSchedulerDetail>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<ActiveInActivePreventiveCommand, PreventiveSchedulerHeader>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));
        }
    }
}