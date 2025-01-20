using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.State.Queries.GetStates
{   
   public class GetStateQuery : IRequest<ApiResponseDTO<List<StateDto>>>;
          
}