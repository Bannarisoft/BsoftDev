using Core.Application.Entity.Queries.GetEntity;
using MediatR;


namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQuery : IRequest<EntityDto>
    {
        public int EntityId { get; set; }
    }
}