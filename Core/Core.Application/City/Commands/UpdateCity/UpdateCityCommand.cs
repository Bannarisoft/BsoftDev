using Core.Application.City.Queries.GetCities;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.City.Commands.UpdateCity
{
       public class UpdateCityCommand : IRequest<ApiResponseDTO<CityDto>> 
       {
                public int Id { get; set; }
                public string? CityCode { get; set; }
                public string? CityName { get; set; }                
                public int StateId { get; set; }
         }
  
}