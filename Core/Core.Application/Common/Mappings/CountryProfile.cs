using AutoMapper;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Commands;
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
             CreateMap<CreateCountryCommand, Countries>();            
            CreateMap<UpdateCountryCommand, Countries>();
            CreateMap<DeleteCountryCommand, CountryDto>();  
            CreateMap<States, CountryDto>();         
        }
    }
}    
