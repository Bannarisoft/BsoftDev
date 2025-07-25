using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.ShiftMasterDetails.Commands.CreateShiftMasterDetail;
using Core.Application.ShiftMasterDetails.Commands.DeleteShiftMasterDetail;
using Core.Application.ShiftMasterDetails.Commands.UpdateShiftMasterDetail;
using Core.Application.ShiftMasterDetails.Queries.GetShiftMasterDetail;
using Core.Application.ShiftMasterDetails.Queries.GetShiftMasterDetailById;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class ShiftMasterDetailProfile : Profile
    {
        public ShiftMasterDetailProfile()
        {
            CreateMap<CreateShiftMasterDetailCommand, ShiftMasterDetail>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted))
            .ForMember(dest => dest.DurationInHours, opt => opt.MapFrom(src => 
            (src.EndTime - src.StartTime).TotalHours));
            
            CreateMap<UpdateShiftMasterDetailCommand, ShiftMasterDetail>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive))
            .ForMember(dest => dest.DurationInHours, opt => opt.MapFrom(src => 
            (src.EndTime - src.StartTime).TotalHours));

            CreateMap<DeleteShiftMasterDetailCommand, ShiftMasterDetail>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
            
            
            CreateMap<ShiftMasterDetail, ShiftMasterDetailByIdDto>();
        }
    }
}