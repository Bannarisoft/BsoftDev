using Core.Application.Common.HttpResponse;
using Core.Application.State.Queries.GetStates;
using MediatR;

namespace Core.Application.State.Queries.GetStateById
{
    public class GetStateByIdQuery : IRequest<ApiResponseDTO<StateDto>>
    {
        public int Id { get; set; }
    }
}