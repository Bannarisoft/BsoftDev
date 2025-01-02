using Core.Application.Country.Queries.GetCountries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Core.Application.Common;

namespace Core.Application.Country.Commands.CreateCountry
{     
     public class CreateCountryCommand :  IRequest<Result<CountryDto>>  
     {
          public string CountryCode { get; set; }=string.Empty;
          public string CountryName { get; set; }  =string.Empty;                   
     }
     

}