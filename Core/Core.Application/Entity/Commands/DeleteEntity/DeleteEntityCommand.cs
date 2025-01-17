using Core.Application.Common;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;

namespace Core.Application.Entity.Commands.DeleteEntity
{
    public class DeleteEntityCommand : IRequest<Result<int>> 
    {
        public int EntityId { get; set; }
        public EntityStatusDto UpdateEntityStatusDto { get; set; }
    }
}