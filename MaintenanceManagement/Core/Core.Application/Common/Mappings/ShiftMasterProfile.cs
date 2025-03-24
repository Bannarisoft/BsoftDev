using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ShiftMasterCQRS.Commands.CreateShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.DeleteShiftMaster;
using Core.Application.ShiftMasterCQRS.Commands.UpdateShiftMaster;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMaster;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterAutoComplete;
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