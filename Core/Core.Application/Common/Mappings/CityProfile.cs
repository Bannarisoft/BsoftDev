using AutoMapper;
using Core.Domain.Entities;
using Core.Application.City.Commands.CreateCity;
using Core.Application.City.Commands.UpdateCity;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Commands.DeleteCity;

namespace Core.Application.Common.Mappings
{
    public class CityProfile : Profile
    {
        
        public CityProfile()
        {                     
            CreateMap<UpdateCityCommand, Cities>();
            CreateMap<DeleteCityCommand, CityDto>();
            CreateMap<CreateCityCommand, Cities>(); 
            CreateMap<States, CityDto>();
        }
    }
}    
