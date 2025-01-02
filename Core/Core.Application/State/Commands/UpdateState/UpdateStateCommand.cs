using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.State.Commands.UpdateState
{
       public class UpdateStateCommand : IRequest<Result<StateDto>>
       {
                public int Id { get; set; }
                public string StateCode { get; set; }=string.Empty;
                public string StateName { get; set; }=string.Empty;       
                public int CountryId { get; set; }         
         }
  
}