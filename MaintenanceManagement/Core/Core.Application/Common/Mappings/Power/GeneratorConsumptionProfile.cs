using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Power.GeneratorConsumption.Command;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.Power
{
    public class GeneratorConsumptionProfile : Profile
    {
        public GeneratorConsumptionProfile()
        {
            CreateMap<Core.Domain.Entities.Power.GeneratorConsumption, GeneratorConsumptionDto>();
            CreateMap<CreateGeneratorConsumptionCommand, Core.Domain.Entities.Power.GeneratorConsumption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Energy, opt => opt.Ignore())
                .ForMember(dest => dest.RunningHours, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
        }
    }
}