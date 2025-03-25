using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Application.CostCenter.Command.DeleteCostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class CostCenterProfile : Profile
    {
        public CostCenterProfile()
        {
           CreateMap<Core.Domain.Entities.CostCenter,CostCenterDto>();
           CreateMap<Core.Domain.Entities.CostCenter, CostCenterAutoCompleteDto>(); 
           CreateMap<CreateCostCenterCommand, Core.Domain.Entities.CostCenter>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CostCenterCode, opt => opt.MapFrom(src => src.CostCenterCode))
                .ForMember(dest => dest.CostCenterName, opt => opt.MapFrom(src => src.CostCenterName))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.ResponsiblePerson))
                .ForMember(dest => dest.BudgetAllocated, opt => opt.MapFrom(src => src.BudgetAllocated))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));


            CreateMap<UpdateCostCenterCommand, Core.Domain.Entities.CostCenter>()
                .ForMember(dest => dest.CostCenterCode, opt => opt.Ignore())
                .ForMember(dest => dest.CostCenterName, opt => opt.MapFrom(src => src.CostCenterName))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.EffectiveDate))
                .ForMember(dest => dest.ResponsiblePerson, opt => opt.MapFrom(src => src.ResponsiblePerson))
                .ForMember(dest => dest.BudgetAllocated, opt => opt.MapFrom(src => src.BudgetAllocated))
                .ForMember(dest => dest.Remarks, opt => opt.MapFrom(src => src.Remarks))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));


              CreateMap<DeleteCostCenterCommand, Core.Domain.Entities.CostCenter>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));    
        }
    }
}