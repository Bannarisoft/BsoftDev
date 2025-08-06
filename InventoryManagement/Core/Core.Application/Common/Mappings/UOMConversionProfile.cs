using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.UOMConversion.Command.CreateUOMConversion;
using Core.Application.UOMConversion.Command.DeleteUOMConversion;
using Core.Application.UOMConversion.Command.UpdateUOMConversion;
using Core.Application.UOMConversion.Queries.GetAllUOMConversion;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class UOMConversionProfile : Profile
    {
        public UOMConversionProfile()
        {


            CreateMap<Core.Domain.Entities.UOMConversion, UOMConversionDto>();

            CreateMap<CreateUOMConversionCommand, Core.Domain.Entities.UOMConversion>()
             .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
             .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));

            CreateMap<UpdateUOMConversionCommand, Core.Domain.Entities.UOMConversion>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteUOMConversionCommand, Core.Domain.Entities.UOMConversion>()
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));

        }
        
    }
}