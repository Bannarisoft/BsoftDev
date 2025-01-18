using Core.Application.Common;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;


namespace Core.Application.Entity.Queries.GetEntityById
{
    public class GetEntityByIdQuery :IRequest<Result<List<EntityDto>>>
    {
        public int EntityId { get; set; }
    }
}