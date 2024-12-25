using AutoMapper;
using BSOFT.Application.Country.Commands;
using BSOFT.Application.Country.DTO;
using BSOFT.Domain.Entities;

namespace BSOFT.Application.Common.Mappings
{    
    public class CountryProfile  : Profile
    {
        
        public CountryProfile()
        {
            CreateMap<CreateCountryCommand, Countries>()
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryName));
                
            
            CreateMap<UpdateCountryCommand, Countries>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Example for handling specific properties
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryName))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode));
            

            // Map CountryEntity to CountryDto
            CreateMap<Countries, CountryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryName));          
        }
    }
}    
