using BSOFT.Application.Country.Queries.GetCountries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BSOFT.Application.Country.Commands.CreateCountry
{     
     public class CreateCountryCommand : IRequest<CountryDto>
     {
          public string CountryCode { get; set; }=string.Empty;
          public string CountryName { get; set; }  =string.Empty;        
           public byte  IsActive { get; set; }
     }
     

}