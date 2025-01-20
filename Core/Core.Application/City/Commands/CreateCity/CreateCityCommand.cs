using Core.Application.City.Queries.GetCities;
using MediatR;
using Core.Application.Common.HttpResponse;

namespace Core.Application.City.Commands.CreateCity
{     
    public class CreateCityCommand : IRequest<ApiResponseDTO<CityDto>>  
    {
        public int StateId { get; set; }
        public string CityCode { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;        
    }
}