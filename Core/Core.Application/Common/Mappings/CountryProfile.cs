using AutoMapper;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Application.Country.Commands.UpdateCountry;
using Core.Application.Country.Commands.DeleteCountry;

namespace Core.Application.Common.Mappings
{    
        public class CountryProfile  : Profile
    {        
        public CountryProfile()
        {
            CreateMap<CreateCountryCommand, Countries>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore the ID if it's auto-generated
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.CountryName))
            .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.CountryCode))
            .ForMember(dest => dest.States, opt => opt.Ignore()); // Ignore States if not provided in the command          
            CreateMap<UpdateCountryCommand, Countries>();
            CreateMap<DeleteCountryCommand, CountryDto>();  
            CreateMap<States, CountryDto>();         
        }
    }
}    
