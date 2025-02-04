using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQuery : IRequest<ApiResponseDTO<List<GetEntityDTO>>>;  
}