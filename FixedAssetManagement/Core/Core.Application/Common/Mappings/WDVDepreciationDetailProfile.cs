using AutoMapper;
using Core.Application.WDVDepreciation.Commands.CreateDepreciation;
using Core.Application.WDVDepreciation.Commands.DeleteDepreciation;
using Core.Application.WDVDepreciation.Commands.LockDepreciation;
using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;
using Core.Application.WDVDepreciation.Queries.GetDepreciation;
using Core.Domain.Entities;

namespace Core.Application.Common.Mappings 
{
    public class WDVDepreciationDetailProfile : Profile    
    {
        public WDVDepreciationDetailProfile()
        {             
            CreateMap<DeleteDepreciationCommand, CalculationDepreciationDto>()
                .ForMember(dest => dest.FinYear, opt => opt.MapFrom(src => src.FinYearId));
            CreateMap<LockDepreciationCommand, CalculationDepreciationDto>()
                .ForMember(dest => dest.FinYear, opt => opt.MapFrom(src => src.FinYearId));
            CreateMap<CreateDepreciationCommand, WDVDepreciationDetail>();
            CreateMap<WDVDepreciationDetail, CalculationDepreciationDto>();                         
            CreateMap<WDVDepreciationDetail, CalculationDepreciationDto>();                            
        }             
    }
}