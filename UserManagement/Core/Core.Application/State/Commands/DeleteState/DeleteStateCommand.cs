using Core.Application.Common.HttpResponse;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.State.Commands.DeleteState
{
       public class DeleteStateCommand :  IRequest<ApiResponseDTO<StateDto>>  
       {
                public int Id { get; set; }                
       }
    
}