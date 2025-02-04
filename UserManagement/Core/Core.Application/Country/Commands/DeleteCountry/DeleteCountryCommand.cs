using Core.Application.Common.HttpResponse;
using Core.Application.Country.Queries.GetCountries;
using MediatR;

namespace Core.Application.Country.Commands.DeleteCountry
{
       public class DeleteCountryCommand :  IRequest<ApiResponseDTO<CountryDto>>  
       {
                public int Id { get; set; }                
       }
    
}