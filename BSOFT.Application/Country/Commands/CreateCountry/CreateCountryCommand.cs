using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BSOFT.Application.Country.DTO;
using MediatR;

namespace BSOFT.Application.Country.Commands
{     
     //public record CreateCountryCommand(string CountryCode, string CountryName) : IRequest<CountryDto>;
     public class CreateCountryCommand : IRequest<CountryDto>
     {
          public string CountryCode { get; set; }=string.Empty;
          public string CountryName { get; set; }  =string.Empty;        
           public byte  IsActive { get; set; }
     }
     

}