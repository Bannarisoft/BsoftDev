using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BSOFT.Application.Country.Commands
{

        //public record UpdateCountryCommand(int Id, string CountryCode, string CountryName,byte IsActive) : IRequest<int>;
         public class UpdateCountryCommand : IRequest<int>
         {
                public int Id { get; set; }
                public string CountryCode { get; set; }=string.Empty;
                public string CountryName { get; set; }=string.Empty;
                public byte IsActive { get; set; }
         }
  
}