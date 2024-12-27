using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.Country.Commands.UpdateCountry
{
       public class UpdateCountryCommand : IRequest<int>
       {
                public int Id { get; set; }
                public string CountryCode { get; set; }=string.Empty;
                public string CountryName { get; set; }=string.Empty;
                public byte IsActive { get; set; }
         }
  
}