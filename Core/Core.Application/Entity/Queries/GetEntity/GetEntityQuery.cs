using MediatR;

namespace Core.Application.Entity.Queries.GetEntity
{
    public class GetEntityQuery : IRequest<List<EntityDto>>;  
}