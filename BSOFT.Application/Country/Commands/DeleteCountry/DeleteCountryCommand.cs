using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace BSOFT.Application.Country.Commands
{
        
        //public record DeleteCountryCommand(int Id, byte IsActive) : IRequest<int>;
         public class DeleteCountryCommand : IRequest<int>
         {
                public int Id { get; set; }
                public byte IsActive { get; set; }
         }
    
}