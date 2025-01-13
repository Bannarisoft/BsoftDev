using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using MediatR;

namespace Core.Application.State.Queries.GetStates
{   
   public class GetStateQuery : IRequest<Result<List<StateDto>>>;
          
}