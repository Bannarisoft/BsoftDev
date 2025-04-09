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
            CreateMap<CreatePreventiveSchedulerCommand, PreventiveSchedulerHdr>()
            .ForMember(dest => dest.PreventiveSchedulerActivities, opt => opt.MapFrom(src => src.Activity))
            .ForMember(dest => dest.PreventiveSchedulerItems, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.DownTimeEstimateHrs, opt => opt.MapFrom(src => src.DownTimeEstimateHrs))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

             CreateMap<PreventiveSchedulerActivityDto, PreventiveSchedulerActivity>()
             .ForMember(dest => dest.EstimatedTimeHrs, opt => opt.MapFrom(src => src.EstimatedTimeHrs));
             CreateMap<PreventiveSchedulerItemsDto, PreventiveSchedulerItems>()
             .ForMember(dest => dest.OldItemId, opt => opt.MapFrom(src => src.ItemId));

             CreateMap<UpdatePreventiveSchedulerCommand, PreventiveSchedulerHdr>()
            .ForMember(dest => dest.PreventiveSchedulerActivities, opt => opt.MapFrom(src => src.Activity))
             .ForMember(dest => dest.PreventiveSchedulerItems, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<PreventiveSchedulerActivityUpdateDto, PreventiveSchedulerActivity>();
            CreateMap<PreventiveSchedulerItemUpdateDto, PreventiveSchedulerItems>();

            CreateMap<DeletePreventiveSchedulerCommand, PreventiveSchedulerHdr>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));

             CreateMap<PreventiveSchedulerHdr, PreventiveSchedulerHdrByIdDto>()
            .ForMember(dest => dest.Activity, opt => opt.MapFrom(src => src.PreventiveSchedulerActivities)) 
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PreventiveSchedulerItems));

            CreateMap<PreventiveSchedulerActivity, PreventiveSchedulerActivityByIdDto>();
            CreateMap<PreventiveSchedulerItems, PreventiveSchedulerItemByIdDto>();
        }
    }
}