using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Power.Feeder.Command.CreateFeeder;
using Core.Application.Power.Feeder.Command.DeleteFeeder;
using Core.Application.Power.Feeder.Command.UpdateFeeder;
using Core.Application.Power.Feeder.Queries.GetFeeder;
using Core.Application.Power.Feeder.Queries.GetFeederAutoComplete;
using Core.Application.Power.Feeder.Queries.GetFeederById;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.Power
{
    public class FeederProfile : Profile
    {

        public FeederProfile()
        {
            CreateMap<Core.Domain.Entities.Power.Feeder, GetFeederDto>();
            CreateMap<Core.Domain.Entities.Power.Feeder, GetFeederByIdDto>()                      
             .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == Status.Active));

            CreateMap<CreateFeederCommand, Core.Domain.Entities.Power.Feeder>()
            .ForMember(dest => dest.HighPriority, opt => opt.MapFrom(src => src.HighPriority == 1 ? true : false));

            CreateMap<UpdateFeederCommand, Core.Domain.Entities.Power.Feeder>()               
                .ForMember(dest => dest.HighPriority, opt => opt.MapFrom(src => src.HighPriority == 1 ? true : false))
             .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == 1 ? Status.Active : Status.Inactive));

            CreateMap<DeleteFeederCommand, Core.Domain.Entities.Power.Feeder>()
             .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));
             
              CreateMap<Core.Domain.Entities.Power.Feeder, GetFeederAutoCompleteDto>();

        }
        
    }
}