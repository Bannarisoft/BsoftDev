using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ShiftMasters.Commands.CreateShiftMaster;
using Core.Application.ShiftMasters.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasters.Commands.UpdateShiftMaster;
using Core.Application.ShiftMasters.Queries.GetShiftMaster;
using Core.Application.ShiftMasters.Queries.GetShiftMasterAutoComplete;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class ShiftMasterProfile : Profile
    {
        public ShiftMasterProfile()
        {
            CreateMap<CreateShiftMasterCommand, Core.Domain.Entities.ShiftMaster>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
            
            CreateMap<UpdateShiftMasterCommand, Core.Domain.Entities.ShiftMaster>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteShiftMasterCommand, Core.Domain.Entities.ShiftMaster>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
            
            CreateMap<Core.Domain.Entities.ShiftMaster, ShiftMasterDTO>();
            CreateMap<Core.Domain.Entities.ShiftMaster, ShiftMasterAutoCompleteDTO>();
        }
    }
}