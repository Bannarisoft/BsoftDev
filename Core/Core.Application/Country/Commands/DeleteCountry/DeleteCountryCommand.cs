using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.Country.Commands
{
       public class DeleteCountryCommand : IRequest<int>
       {
                public int Id { get; set; }
                public byte IsActive { get; set; }
       }
    
}