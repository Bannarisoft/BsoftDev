using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.WorkCenter.Command.CreateWorkCenter;
using Core.Application.WorkCenter.Command.DeleteWorkCenter;
using Core.Application.WorkCenter.Command.UpdateWorkCenter;
using Core.Application.WorkCenter.Queries.GetWorkCenter;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class WorkCenterProfile : Profile
    {
        public WorkCenterProfile()
        {
              CreateMap<Core.Domain.Entities.WorkCenter,WorkCenterDto>();
              CreateMap<Core.Domain.Entities.WorkCenter, WorkCenterAutoCompleteDto>();
              CreateMap<CreateWorkCenterCommand, Core.Domain.Entities.WorkCenter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WorkCenterCode, opt => opt.MapFrom(src => src.WorkCenterCode))
                .ForMember(dest => dest.WorkCenterName, opt => opt.MapFrom(src => src.WorkCenterName))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

              CreateMap<UpdateWorkCenterCommand, Core.Domain.Entities.WorkCenter>()
                .ForMember(dest => dest.WorkCenterCode, opt => opt.Ignore())
                .ForMember(dest => dest.WorkCenterName, opt => opt.MapFrom(src => src.WorkCenterName))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

              CreateMap<DeleteWorkCenterCommand, Core.Domain.Entities.WorkCenter>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));  
        }
    }
}