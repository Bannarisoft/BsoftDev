using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.MaintenanceCategory.Command.DeleteMaintenanceCategory;
using Core.Application.MaintenanceType.Command.CreateMaintenanceType;
using Core.Application.MaintenanceType.Command.DeleteMaintenanceType;
using Core.Application.MaintenanceType.Command.UpdateMaintenanceType;
using Core.Application.MaintenanceType.Queries.GetMaintenanceType;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class MaintenanceTypeProfile : Profile
    {
         public MaintenanceTypeProfile()
        {
           CreateMap<Core.Domain.Entities.MaintenanceType,MaintenanceTypeDto>();
           CreateMap<Core.Domain.Entities.MaintenanceType, MaintenanceTypeAutoCompleteDto>(); 
           CreateMap<CreateMaintenanceTypeCommand, Core.Domain.Entities.MaintenanceType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));


            CreateMap<UpdateMaintenanceTypeCommand, Core.Domain.Entities.MaintenanceType>()
                .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TypeName))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));


              CreateMap<DeleteMaintenanceTypeCommand, Core.Domain.Entities.MaintenanceType>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted)); 
        }
    }
}