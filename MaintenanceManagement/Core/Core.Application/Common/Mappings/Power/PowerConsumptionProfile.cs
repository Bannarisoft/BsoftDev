using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Power.PowerConsumption.Command.CreatePowerConsumption;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.Power
{
    public class PowerConsumptionProfile : Profile
    {
        public PowerConsumptionProfile()
        {
            CreateMap<Core.Domain.Entities.Power.PowerConsumption, PowerConsumptionDto>();
            CreateMap<CreatePowerConsumptionCommand, Core.Domain.Entities.Power.PowerConsumption>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TotalUnits, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));
        }
    }
}