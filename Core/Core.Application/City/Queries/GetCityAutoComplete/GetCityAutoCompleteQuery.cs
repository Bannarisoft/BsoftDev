using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.City.Queries.GetCities;
using MediatR;


namespace Core.Application.City.Queries.GetCityAutoComplete
{
    public class GetCityAutoCompleteQuery : IRequest<List<CityDto>>
    {
        public string SearchPattern { get; set; }=string.Empty;        
    }
}