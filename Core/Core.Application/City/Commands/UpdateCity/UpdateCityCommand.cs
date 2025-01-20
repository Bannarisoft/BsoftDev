using Core.Application.City.Queries.GetCities;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.City.Commands.UpdateCity
{
       public class UpdateCityCommand : IRequest<ApiResponseDTO<CityDto>> 
       {
                public int Id { get; set; }
                public string CityCode { get; set; }=string.Empty;
                public string CityName { get; set; }=string.Empty;                
                public int StateId { get; set; }
         }
  
}