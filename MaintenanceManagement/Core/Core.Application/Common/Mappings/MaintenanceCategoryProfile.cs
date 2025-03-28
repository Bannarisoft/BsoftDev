using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.MaintenanceCategory.Command.CreateMaintenanceCategory;
using Core.Application.MaintenanceCategory.Command.DeleteMaintenanceCategory;
using Core.Application.MaintenanceCategory.Command.UpdateMaintenanceCategory;
using Core.Application.MaintenanceCategory.Queries.GetMaintenanceCategory;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class MaintenanceCategoryProfile  : Profile
    {
        public MaintenanceCategoryProfile()
        {
           CreateMap<Core.Domain.Entities.MaintenanceCategory,MaintenanceCategoryDto>();
           CreateMap<Core.Domain.Entities.MaintenanceCategory, MaintenanceCategoryAutoCompleteDto>(); 
           CreateMap<CreateMaintenanceCategoryCommand, Core.Domain.Entities.MaintenanceCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));


            CreateMap<UpdateMaintenanceCategoryCommand, Core.Domain.Entities.MaintenanceCategory>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));


              CreateMap<DeleteMaintenanceCategoryCommand, Core.Domain.Entities.MaintenanceCategory>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 
        }
    }
}