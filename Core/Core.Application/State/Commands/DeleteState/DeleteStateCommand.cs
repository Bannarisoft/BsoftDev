using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.State.Commands.DeleteState
{
       public class DeleteStateCommand :  IRequest<Result<StateDto>>  
       {
                public int Id { get; set; }                
       }
    
}