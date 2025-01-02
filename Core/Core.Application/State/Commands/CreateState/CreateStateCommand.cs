using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common;

namespace Core.Application.State.Commands.CreateState
{     
     public class CreateStateCommand : IRequest<Result<StateDto>>  
     {
          public string StateCode { get; set; }=string.Empty;
          public string StateName { get; set; }  =string.Empty;   
          public int CountryId { get; set; }                
     }
     

}