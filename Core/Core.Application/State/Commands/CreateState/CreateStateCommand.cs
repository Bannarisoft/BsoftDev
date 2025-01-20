using MediatR;
using Core.Application.State.Queries.GetStates;
using Core.Application.Common.HttpResponse;

namespace Core.Application.State.Commands.CreateState
{     
     public class CreateStateCommand : IRequest<ApiResponseDTO<StateDto>>  
     {
          public string StateCode { get; set; }=string.Empty;
          public string StateName { get; set; }  =string.Empty;   
          public int CountryId { get; set; }                
     }    

}