using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Entity.Queries.GetEntity;
using MediatR;

namespace Core.Application.Entity.Commands.DeleteEntity
{
    public class DeleteEntityCommand : IRequest<ApiResponseDTO<int>> 
    {
        public int EntityId { get; set; }
    
    }
}