using Core.Application.Common;
using MediatR;

namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQuery : IRequest<Result<List<EntityDto>>>;  
}