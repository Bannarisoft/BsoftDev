using Core.Application.Common.HttpResponse;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.State.Commands.UpdateState
{
       public class UpdateStateCommand : IRequest<ApiResponseDTO<StateDto>>
       {
              public int Id { get; set; }
              public string? StateCode { get; set; }
              public string? StateName { get; set; }
              public int CountryId { get; set; }         
         }
  
}